using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealPlannerApp.Shared.Models
{
    public class MealPlanEntry
    {
        public int Id { get; set; }

        [Required]
        public DayOfWeek Day { get; set; } // Hétfő, Kedd, stb.

        [Required]
        public MealType Type { get; set; } // Reggeli, Ebéd, Vacsora

        [Required]
        public int RecipeId { get; set; }

        [ForeignKey("RecipeId")]
        public Recipe? Recipe { get; set; } // Kapcsolat a recepthez
    }
}