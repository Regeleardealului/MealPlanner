using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; 

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

        [JsonIgnore]
        public Recipe? Recipe { get; set; }
    }
}
