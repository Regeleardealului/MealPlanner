using MealPlannerApp.Data;
using MealPlannerApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShoppingListController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShoppingListController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingListItem>>> GetShoppingList()
        {
            var mealPlanEntries = await _context.MealPlanEntries
                .Include(entry => entry.Recipe)
                    .ThenInclude(recipe => recipe!.Ingredients)
                .ToListAsync();

            if (!mealPlanEntries.Any())
            {
                return Ok(new List<ShoppingListItem>());
            }

            var allIngredients = mealPlanEntries
                .Where(entry => entry.Recipe is not null)
                .SelectMany(entry => entry.Recipe!.Ingredients)
                .Where(ingredient => ingredient is not null);

            var shoppingList = allIngredients
                .GroupBy(ingredient => new { ingredient.Name, ingredient.Unit })
                .Select(group => new ShoppingListItem
                {
                    Name = group.Key.Name,
                    Unit = group.Key.Unit,
                    TotalQuantity = group.Sum(item => item.Quantity),
                    Category = GetCategoryForIngredient(group.Key.Name) 
                })
                .OrderBy(item => item.Category) 
                .ThenBy(item => item.Name)    
                .ToList();

            return Ok(shoppingList);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearShoppingList()
        {
            var currentEntries = await _context.MealPlanEntries.ToListAsync();

            if (currentEntries.Any())
            {
                _context.MealPlanEntries.RemoveRange(currentEntries);
                await _context.SaveChangesAsync();
            }

            return Ok(); 
        }

        private string GetCategoryForIngredient(string ingredientName)
        {
            var name = ingredientName.ToLower();

            if (name.Contains("hús") || name.Contains("csirke") || name.Contains("marha") || name.Contains("sertés") || name.Contains("szalonna") || name.Contains("kolbász"))
                return "Húsfélék";
            if (name.Contains("sajt") || name.Contains("tejföl") || name.Contains("túró") || name.Contains("tej"))
                return "Tejtermékek";
            if (name.Contains("tojás"))
                return "Tojás";
            if (name.Contains("hagyma") || name.Contains("paradicsom") || name.Contains("saláta") || name.Contains("krumpli") || name.Contains("burgonya") || name.Contains("répa") || name.Contains("gyökér") || name.Contains("zöldborsó") || name.Contains("káposzta") || name.Contains("fokhagyma"))
                return "Zöldségek";
            if (name.Contains("tészta") || name.Contains("liszt") || name.Contains("rizs"))
                return "Pékáru & Tésztafélék";
            if (name.Contains("lencse") || name.Contains("cukor"))
                return "Tartós élelmiszer";
            if (name.Contains("olaj") || name.Contains("só") || name.Contains("babérlevél") || name.Contains("fűszer") || name.Contains("sáfrány") || name.Contains("zsír"))
                return "Fűszerek & Olajok";

            return "Egyéb";
        }
    }
}
