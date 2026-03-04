using MealPlannerApp.Data;
using MealPlannerApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MealPlannerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Csak bejelentkezett felhasználók babrálhatják a profiljukat!
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- DIÉTA BEÁLLÍTÁSOK LEKÉRÉSE ---
        [HttpGet("preferences")]
        public async Task<ActionResult<UserPreferencesDto>> GetPreferences()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var pref = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);

            if (pref == null)
            {
                return Ok(new UserPreferencesDto()); // Ha még nincs, üres formot adunk
            }

            return Ok(new UserPreferencesDto
            {
                IsVegan = pref.IsVegan,
                IsVegetarian = pref.IsVegetarian,
                IsGlutenFree = pref.IsGlutenFree,
                IsLactoseFree = pref.IsLactoseFree,
                AllergiesOrDislikes = pref.AllergiesOrDislikes
            });
        }

        // --- DIÉTA BEÁLLÍTÁSOK MENTÉSE ---
        [HttpPut("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UserPreferencesDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var pref = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);

            if (pref == null)
            {
                // Ha még sosem mentett beállítást, létrehozzuk
                pref = new UserPreference { UserId = userId! };
                _context.UserPreferences.Add(pref);
            }

            pref.IsVegan = dto.IsVegan;
            pref.IsVegetarian = dto.IsVegetarian;
            pref.IsGlutenFree = dto.IsGlutenFree;
            pref.IsLactoseFree = dto.IsLactoseFree;
            pref.AllergiesOrDislikes = dto.AllergiesOrDislikes;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // --- JELSZÓ CSERE ---
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null) return NotFound("Felhasználó nem található.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return Ok();
        }
    }
}