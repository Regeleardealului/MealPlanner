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
    [Authorize] 
    public class RecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

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

            var recipeToUpdate = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (recipeToUpdate == null)
            {
                return NotFound();
            }

            recipeToUpdate.Title = recipe.Title;
            recipeToUpdate.Description = recipe.Description;
            recipeToUpdate.ImageUrl = recipe.ImageUrl;
            recipeToUpdate.PrepTimeInMinutes = recipe.PrepTimeInMinutes;
            recipeToUpdate.Difficulty = recipe.Difficulty;
            recipeToUpdate.Instructions = recipe.Instructions;
            recipeToUpdate.Tags = recipe.Tags;

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
