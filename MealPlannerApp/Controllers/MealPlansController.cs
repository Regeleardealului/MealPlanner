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

        // GET: api/mealplans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MealPlanEntry>>> GetMealPlan()
        {
            return await _context.MealPlanEntries.Include(e => e.Recipe).ToListAsync();
        }

        // POST: api/mealplans
        [HttpPost]
        public async Task<IActionResult> PostMealPlanEntry([FromBody] MealPlanEntry mealPlanEntry)
        {
            // Ellenőrizzük, hogy a bejövő adat érvényes-e
            if (mealPlanEntry == null || mealPlanEntry.RecipeId <= 0)
            {
                return BadRequest("Invalid meal plan entry data.");
            }

            // Először töröljük, ha az adott helyen (nap, étkezés) már van bejegyzés
            var existingEntry = await _context.MealPlanEntries
                .FirstOrDefaultAsync(e => e.Day == mealPlanEntry.Day && e.Type == mealPlanEntry.Type);

            if (existingEntry != null)
            {
                _context.MealPlanEntries.Remove(existingEntry);
            }

            // Hozzáadjuk az új bejegyzést
            // Fontos: a 'Recipe' navigációs property-t null-ra állítjuk, mert csak az Id-t akarjuk menteni.
            mealPlanEntry.Recipe = null;
            _context.MealPlanEntries.Add(mealPlanEntry);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Adatbázis hiba esetén részletesebb hibát adunk vissza
                return StatusCode(500, $"Error saving to database: {ex.InnerException?.Message ?? ex.Message}");
            }

            // Ha a mentés sikeres, nem küldünk vissza bonyolult objektumot, csak egy "OK" választ.
            // A kliens majd úgyis újra lekéri a friss adatokat.
            return Ok();
        }

        // DELETE: api/mealplans/5
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