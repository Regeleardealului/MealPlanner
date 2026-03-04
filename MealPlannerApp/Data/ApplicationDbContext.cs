using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MealPlannerApp.Shared.Models;

namespace MealPlannerApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<MealPlanEntry> MealPlanEntries { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Nagyon fontos: Identity használatakor először az alap osztály 
            // OnModelCreating metódusát kell meghívni!
            base.OnModelCreating(modelBuilder);

            // Ez a rész konvertálja a List<string>-et egyetlen stringgé és vissza
            modelBuilder.Entity<Recipe>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            // Adatbázis inicializálása adatokkal
            modelBuilder.Seed();
        }
    }
}