using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // EZT A SORT ADJUK HOZZÁ!

namespace MealPlannerApp.Shared.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public string Unit { get; set; } = string.Empty;

        public int RecipeId { get; set; }

        // EZ AZ ATTRIBÚTUM MEGOLD MINDENT:
        // Azt mondja a rendszernek, hogy amikor JSON-t készít,
        // ezt a visszafelé mutató hivatkozást hagyja figyelmen kívül.
        [JsonIgnore]
        public Recipe? Recipe { get; set; }
    }
}