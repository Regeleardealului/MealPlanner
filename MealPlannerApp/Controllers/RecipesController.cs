using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealPlannerApp.Data;
using MealPlannerApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MealPlannerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Az egész kontrollert védjük: csak bejelentkezett felhasználó érheti el
    public class RecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Segédmetódus a bejelentkezett felhasználó ID-jának lekéréséhez
        private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Csak a saját recepteket adjuk vissza a hozzávalókkal együtt
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Keresés ID ÉS UserId alapján a biztonság érdekében
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (recipe == null) { return NotFound(); }
            return recipe;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, Recipe recipe)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (id != recipe.Id)
            {
                return BadRequest();
            }

            // Betöltjük a módosítandó receptet, ellenőrizve a tulajdonjogot
            var recipeToUpdate = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (recipeToUpdate == null)
            {
                return NotFound();
            }

            // Alap adatok frissítése
            recipeToUpdate.Title = recipe.Title;
            recipeToUpdate.Description = recipe.Description;
            recipeToUpdate.ImageUrl = recipe.ImageUrl;
            recipeToUpdate.PrepTimeInMinutes = recipe.PrepTimeInMinutes;
            recipeToUpdate.Difficulty = recipe.Difficulty;
            recipeToUpdate.Instructions = recipe.Instructions;
            recipeToUpdate.Tags = recipe.Tags;

            // Hozzávalók frissítése (régi törlése, új hozzáadása)
            recipeToUpdate.Ingredients.Clear();
            foreach (var ingredientFromClient in recipe.Ingredients)
            {
                recipeToUpdate.Ingredients.Add(new Ingredient
                {
                    Name = ingredientFromClient.Name,
                    Quantity = ingredientFromClient.Quantity,
                    Unit = ingredientFromClient.Unit
                });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error saving to database.");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // A recepthez hozzárendeljük a bejelentkezett felhasználót
            recipe.UserId = userId;

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Csak akkor törölhető, ha az ID stimmel ÉS a felhasználó a tulajdonos
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (recipe == null) { return NotFound(); }

            _context.Ingredients.RemoveRange(recipe.Ingredients);
            _context.Recipes.Remove(recipe);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool RecipeExists(int id)
        {
            var userId = GetUserId();
            return _context.Recipes.Any(e => e.Id == id && e.UserId == userId);
        }
    }
}