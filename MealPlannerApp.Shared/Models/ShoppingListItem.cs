namespace MealPlannerApp.Shared.Models
{
    public class ShoppingListItem
    {
        public string Name { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}