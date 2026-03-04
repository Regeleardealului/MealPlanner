using MealPlannerApp.Data;
using MealPlannerApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecommendationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecommendations([FromQuery] double maxPrepTime, [FromQuery] string? tag)
        {
            var query = _context.Recipes.AsQueryable();

            if (maxPrepTime > 0)
            {
                query = query.Where(r => r.PrepTimeInMinutes <= maxPrepTime);
            }

            var recipesFromDb = await query.ToListAsync();

            if (!string.IsNullOrEmpty(tag))
            {
                recipesFromDb = recipesFromDb.Where(r => r.Tags != null && r.Tags.Contains(tag)).ToList();
            }

            return Ok(recipesFromDb);
        }
    }
}
