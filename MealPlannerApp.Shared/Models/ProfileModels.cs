using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealPlannerApp.Shared.Models
{
    public class UserPreferencesDto
    {
        public bool IsVegan { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsLactoseFree { get; set; }

        public string AllergiesOrDislikes { get; set; } = string.Empty;
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}

