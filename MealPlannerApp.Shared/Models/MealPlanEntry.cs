using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealPlannerApp.Shared.Models
{
    public class MealPlanEntry
    {
        public int Id { get; set; }

        [Required]
        public DayOfWeek Day { get; set; } 

        [Required]
        public MealType Type { get; set; } 

        [Required]
        public int RecipeId { get; set; }

        [ForeignKey("RecipeId")]
        public Recipe? Recipe { get; set; } 
    }

}
