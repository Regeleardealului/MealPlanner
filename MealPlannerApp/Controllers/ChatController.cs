using MealPlannerApp.Data;
using MealPlannerApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MealPlannerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string? _openAiApiKey;

        public ChatController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _openAiApiKey = configuration["OpenAiApiKey"];
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(userQuestion) || string.IsNullOrWhiteSpace(_openAiApiKey))
            {
                return BadRequest("A kérdés vagy az OpenAI API kulcs hiányzik.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("Felhasználó nem azonosítható.");

            var prefs = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);

            var allRecipes = await _context.Recipes
                .Include(r => r.Ingredients)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            var keywords = userQuestion.ToLower().Split(new[] { ' ', ',', '.', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var relevantRecipes = allRecipes
                .Where(r => r.Title != null && keywords.Any(k =>
                    r.Title.ToLower().Contains(k) ||
                    (r.Ingredients != null && r.Ingredients.Any(i => i.Name != null && i.Name.ToLower().Contains(k)))
                ))
                .ToList();

            if (!relevantRecipes.Any())
            {
                relevantRecipes = allRecipes.Take(5).ToList();
            }

            var systemPromptBuilder = new StringBuilder();
            systemPromptBuilder.AppendLine("Te egy profi és barátságos személyes séf és recept-asszisztens vagy.");
            systemPromptBuilder.AppendLine("KIZÁRÓLAG a felhasználó saját, alább megadott receptkönyvében található receptekből dolgozhatsz! Ha egy kért étel nincs a listában, mondd meg őszintén, hogy ez nincs meg a receptkönyvében.");

            if (prefs != null)
            {
                systemPromptBuilder.AppendLine("\nFONTOS SZEMÉLYES BEÁLLÍTÁSOK ÉS ALLERGIÁK (Ezeket SZIGORÚAN tarts tiszteletben!):");
                if (prefs.IsVegan) systemPromptBuilder.AppendLine("- A felhasználó VEGÁN (semmilyen állati eredetű összetevőt nem eszik).");
                if (prefs.IsVegetarian) systemPromptBuilder.AppendLine("- A felhasználó VEGETÁRIÁNUS (húst nem eszik).");
                if (prefs.IsGlutenFree) systemPromptBuilder.AppendLine("- A felhasználó GLUTÉNÉRZÉKENY (szigorúan gluténmentes ételeket ehet csak).");
                if (prefs.IsLactoseFree) systemPromptBuilder.AppendLine("- A felhasználó LAKTÓZÉRZÉKENY (csak laktózmentes ételeket ehet).");


                if (!string.IsNullOrWhiteSpace(prefs.AllergiesOrDislikes))
                {
                    systemPromptBuilder.AppendLine($"- A felhasználó ALLERGIÁS VAGY NEM SZERETI a következőket: {prefs.AllergiesOrDislikes}. Bármilyen receptet ajánlasz, ellenőrizd, hogy NINCSENEK-E benne ezek az alapanyagok!");
                }
            }

            var contextBuilder = new StringBuilder();
            contextBuilder.AppendLine("\nA FELHASZNÁLÓ ELÉRHETŐ RECEPTJEI:\n");
            foreach (var recipe in relevantRecipes)
            {
                contextBuilder.AppendLine($"--- RECEPT: {recipe.Title} ---");
                contextBuilder.AppendLine($"Leírás: {recipe.Description}");
                contextBuilder.AppendLine($"Elkészítési idő: {recipe.PrepTimeInMinutes} perc");
                contextBuilder.AppendLine("Hozzávalók:");
                if (recipe.Ingredients != null)
                {
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        contextBuilder.AppendLine($"- {ingredient.Quantity} {ingredient.Unit} {ingredient.Name}");
                    }
                }
                contextBuilder.AppendLine();
            }

            try
            {
                var client = new OpenAIClient(_openAiApiKey);
                var chatClient = client.GetChatClient("gpt-4o-mini");

                var messages = new List<ChatMessage>
                {
                    ChatMessage.CreateSystemMessage(systemPromptBuilder.ToString()),
                    
                    ChatMessage.CreateUserMessage($"A kérdésem: '{userQuestion}'.\n\nA receptjeim:\n{contextBuilder}")
                };

                var completion = await chatClient.CompleteChatAsync(messages);
                var reply = completion.Value.Content[0].Text;

                return Ok(reply);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hiba történt az AI szolgáltatással való kommunikáció során: {ex.Message}");
            }
        }
    }
}
