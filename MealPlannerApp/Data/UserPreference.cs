using System.ComponentModel.DataAnnotations;

namespace MealPlannerApp.Data
{
    public class UserPreference
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public bool IsVegan { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsLactoseFree { get; set; }
        public string AllergiesOrDislikes { get; set; } = string.Empty;
    }
}
