using MealPlannerApp.Data;
using MealPlannerApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealPlansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MealPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MealPlanEntry>>> GetMealPlan()
        {
            return await _context.MealPlanEntries.Include(e => e.Recipe).ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostMealPlanEntry([FromBody] MealPlanEntry mealPlanEntry)
        {
            if (mealPlanEntry == null || mealPlanEntry.RecipeId <= 0)
            {
                return BadRequest("Invalid meal plan entry data.");
            }

            var existingEntry = await _context.MealPlanEntries
                .FirstOrDefaultAsync(e => e.Day == mealPlanEntry.Day && e.Type == mealPlanEntry.Type);

            if (existingEntry != null)
            {
                _context.MealPlanEntries.Remove(existingEntry);
            }

            mealPlanEntry.Recipe = null;
            _context.MealPlanEntries.Add(mealPlanEntry);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error saving to database: {ex.InnerException?.Message ?? ex.Message}");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMealPlanEntry(int id)
        {
            var entry = await _context.MealPlanEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            _context.MealPlanEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
