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
    [Authorize] // KÖTELEZŐ: Csak bejelentkezve lehessen chatelni!
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

            // 1. Kiderítjük, ki a jelenlegi felhasználó
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("Felhasználó nem azonosítható.");

            // 2. Lekérjük a felhasználó DIÉTA BEÁLLÍTÁSAIT
            var prefs = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);

            // 3. CSAK a felhasználó SAJÁT receptjeit keressük!
            // FIGYELEM: Ha a Recipe osztályodban nincs UserId mező, akkor itt hibát fogsz kapni! 
            // Abban az esetben szólj, és hozzáadjuk a Recipe modellhez!
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
                // Ha nem talál kulcsszót, akkor a saját receptjei közül adunk neki párat kontextusnak
                relevantRecipes = allRecipes.Take(5).ToList();
            }

            // --- Rendszer utasítás (System Prompt) felépítése az allergiákkal ---
            var systemPromptBuilder = new StringBuilder();
            systemPromptBuilder.AppendLine("Te egy profi és barátságos személyes séf és recept-asszisztens vagy.");
            systemPromptBuilder.AppendLine("KIZÁRÓLAG a felhasználó saját, alább megadott receptkönyvében található receptekből dolgozhatsz! Ha egy kért étel nincs a listában, mondd meg őszintén, hogy ez nincs meg a receptkönyvében.");

            // Ha vannak beállításai, azokat BELEVERJÜK az AI fejébe!
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

            // --- Kontextus felépítése a prompthoz ---
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
                // === OpenAI kliens inicializálása ===
                var client = new OpenAIClient(_openAiApiKey);
                var chatClient = client.GetChatClient("gpt-4o-mini");

                // === Üzenetek létrehozása ===
                var messages = new List<ChatMessage>
                {
                    // Itt adjuk át a titkos instrukciót az allergiákkal
                    ChatMessage.CreateSystemMessage(systemPromptBuilder.ToString()),
                    
                    // Itt pedig a konkrét kérdést és a recepteket
                    ChatMessage.CreateUserMessage($"A kérdésem: '{userQuestion}'.\n\nA receptjeim:\n{contextBuilder}")
                };

                // === Válasz lekérése ===
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