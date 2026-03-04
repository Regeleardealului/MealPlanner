using System.ComponentModel.DataAnnotations;

namespace MealPlannerApp.Shared.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public string Instructions { get; set; } = string.Empty;

        public int PrepTimeInMinutes { get; set; }

        public DifficultyLevel Difficulty { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>(); // pl: "olasz", "gyors", "csirke"

        public string UserId { get; set; } = string.Empty;
    }

    public enum DifficultyLevel
    {
        Könnyű,
        Közepes,
        Nehéz
    }
}