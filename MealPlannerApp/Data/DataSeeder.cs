using MealPlannerApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerApp.Data
{
    public static class DataSeeder
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1, Title = "Spagetti Bolognai", Description = "Klasszikus olasz tésztaétel gazdag húsos szósszal.",
                    Instructions = "1. Pirítsd meg a hagymát. 2. Add hozzá a darált húst. 3. Öntsd fel paradicsomszósszal. 4. Főzd 30 percig. 5. Főzd ki a tésztát és tálald.",
                    PrepTimeInMinutes = 45, Difficulty = DifficultyLevel.Közepes, ImageUrl = "images/bolognai.jpg",
                    Tags = new List<string> { "olasz", "tészta", "marha" }
                },
                new Recipe
                {
                    Id = 2, Title = "Csirkés Cézár Saláta", Description = "Grillezett csirkemell, friss saláta, kruton és parmezán, csípős öntettel.",
                    Instructions = "1. Grillezd meg a csirkemellet. 2. Vágd fel a salátát és a csirkét. 3. Keverd össze az öntettel, szórd meg krutonnal és parmezánnal.",
                    PrepTimeInMinutes = 10, Difficulty = DifficultyLevel.Könnyű, ImageUrl = "images/cezar.jpg",
                    Tags = new List<string> { "saláta", "gyors", "csirke", "könnyed" }
                },
                new Recipe
                {
                    Id = 3, Title = "Lencsefőzelék", Description = "Hagyományos magyar főzelék, füstölt kolbásszal vagy anélkül.",
                    Instructions = "1. Áztasd be a lencsét. 2. Főzd puhára a babérlevéllel. 3. Készíts rántást és sűrítsd be vele. 4. Ízesítsd mustárral, ecettel.",
                    PrepTimeInMinutes = 60, Difficulty = DifficultyLevel.Könnyű, ImageUrl = "images/lencsefozelek.jpg",
                    Tags = new List<string> { "magyaros", "főzelék", "vegetáriánus" }
                },
                new Recipe
                {
                    Id = 4, Title = "Turós csusza", Description = "Igazi magyar klasszikus, pirított szalonnával és tejföllel.",
                    Instructions = "1. Főzd ki a csuszatésztát. 2. Kockázd fel a szalonnát és pirítsd zsírjára. 3. Keverd össze a tésztát a túróval, tejföllel és a szalonnapörccel.",
                    PrepTimeInMinutes = 25, Difficulty = DifficultyLevel.Könnyű, ImageUrl = "images/turos_csusza.jpg",
                    Tags = new List<string> { "magyaros", "tészta", "gyors" }
                },
                new Recipe
                {
                    Id = 5, Title = "Lasagne", Description = "Rétegzett olasz tésztaétel, bolognai raguval és besamel mártással.",
                    Instructions = "1. Készítsd el a bolognai ragut. 2. Készítsd el a besamel mártást. 3. Rétegezd a tésztalapokat a raguval, mártással és sajttal. 4. Süsd aranybarnára.",
                    PrepTimeInMinutes = 90, Difficulty = DifficultyLevel.Nehéz, ImageUrl = "images/lasagne.jpg",
                    Tags = new List<string> { "olasz", "tészta", "marha" }
                },
                new Recipe
                {
                    Id = 6, Title = "Rakott krumpli", Description = "Főtt krumpli karikák, kolbásszal, tojással és tejföllel összesütve.",
                    Instructions = "1. Főzd meg a krumplit és a tojást. 2. Karikázd fel őket és a kolbászt. 3. Rétegezd egy tepsibe, locsold meg tejföllel. 4. Süsd készre.",
                    PrepTimeInMinutes = 75, Difficulty = DifficultyLevel.Közepes, ImageUrl = "images/rakott_krumpli.jpg",
                    Tags = new List<string> { "magyaros", "egy-tál" }
                },
                new Recipe
                {
                    Id = 7, Title = "Töltött káposzta", Description = "Savanyú káposzta levelekbe töltött fűszeres darált hús, gerslivel vagy rizzsel.",
                    Instructions = "1. Készítsd el a tölteléket. 2. Töltsd meg a káposztaleveleket. 3. Főzd puhára aprókáposzta ágyon.",
                    PrepTimeInMinutes = 120, Difficulty = DifficultyLevel.Nehéz, ImageUrl = "images/toltott_kaposzta.jpg",
                    Tags = new List<string> { "magyaros", "hagyományos" }
                },
                new Recipe
                {
                    Id = 8, Title = "Paella", Description = "Sáfrányos spanyol rizsétel tenger gyümölcseivel vagy csirkével.",
                    Instructions = "1. Pirítsd meg a hagymát, fokhagymát. 2. Add hozzá a rizst, sáfrányt. 3. Öntsd fel alaplével. 4. Add hozzá a feltéteket és főzd készre.",
                    PrepTimeInMinutes = 50, Difficulty = DifficultyLevel.Közepes, ImageUrl = "images/paella.jpg",
                    Tags = new List<string> { "spanyol", "rizs", "tenger gyümölcsei" }
                },
                new Recipe
                {
                    Id = 9, Title = "Samosa", Description = "Fűszeres zöldségekkel vagy hússal töltött, olajban sült indiai tésztabatyu.",
                    Instructions = "1. Készítsd el a tésztát és a tölteléket. 2. Formázz háromszögeket és töltsd meg őket. 3. Süsd aranybarnára forró olajban.",
                    PrepTimeInMinutes = 60, Difficulty = DifficultyLevel.Közepes, ImageUrl = "images/samosa.jpg",
                    Tags = new List<string> { "indiai", "vegetáriánus", "előétel" }
                },
                new Recipe
                {
                    Id = 11, Title = "Sertéspörkölt nokedlivel", Description = "A magyar konyha egyik klasszikus kedvence, egy gazdag ízvilágú, laktató étel.",
                    Instructions = "1. A vöröshagymát apróra vágjuk, majd a forró zsírban, közepes lángon üvegesre pároljuk egy nagyobb lábasban.\n2. A húst 2-3 cm-es kockákra vágjuk, és a hagymához adjuk. Fehéredésig pirítjuk, majd hozzáadjuk a zúzott fokhagymát is.\n3. Levesszük a tűzről, és belekeverjük az őrölt pirospaprikát, gyorsan elkeverjük, hogy ne égjen meg.\n4. Visszatesszük a tűzre, és hozzáadjuk az apróra vágott paradicsomot és paprikát. \n5. Enyhén sózzuk és borsozzuk, hozzáadjuk a fűszerköményt, majd felöntjük egy kevés vízzel, épp csak annyival, hogy ellepje.\n6. Lefedve, alacsony lángon, gyakori kevergetés mellett, legalább 1,5-2 órán át főzzük, amíg a hús teljesen megpuhul. Ha szükséges, pótoljuk a vizet apránként. Akkor jó, ha a hagyma teljesen szétfőtt, a szaft pedig sűrű és selymes",
                    PrepTimeInMinutes = 50, Difficulty = DifficultyLevel.Nehéz, ImageUrl = "https://cdn.ripost.hu/2022/01/qihckkDe56LsW0LWjeVX8DWOD9iGVFUGnMX402t1oA0/fill/2560/1707/no/1/aHR0cHM6Ly9jbXNjZG4uYXBwLmNvbnRlbnQucHJpdmF0ZS9jb250ZW50LzY1N2QxZjVmNmU3ODQ1NjQ5N2EyOTUwZmY5ZGFjNzJh.jpg",
                    Tags = new List<string>()
                },
                new Recipe
                {
                    Id = 12, Title = "Gulyásleves", Description = "Tartalmas, paprikás leves messze több, mint egy egyszerű fogás: a magyar kultúra és történelem szerves része.",
                    Instructions = "1. Készíts egy pörköltalapot zsíron dinsztelt hagymából és fűszerpaprikából.\n2. Ezen pirítsd meg a felkockázott marhahúst, majd kevés vízzel párold majdnem teljesen puhára.\n3. Add hozzá a zöldségeket, később a burgonyát, öntsd fel az egészet vízzel, majd a végén főzd bele a csipetkét is, amíg minden megpuhul.",
                    PrepTimeInMinutes = 150, Difficulty = DifficultyLevel.Nehéz, ImageUrl = "https://image-api.nosalty.hu/nosalty/images/recipes/SO/9o/tradicionalis-gulyasleves.jpg?w=1800&fit=crop&s=c4e4543b0cac305f2f0f8124ca4eeb22",
                    Tags = new List<string>()
                },
                new Recipe
                {
                    Id = 13, Title = "Lángos", Description = "A magyar gasztronómia egyik legnépszerűbb és legismertebb sült tésztája.",
                    Instructions = "1. Összeállítás és kelesztés: kb. 1 - 1,5 óra\n2. Sütés: kb. 20-30 perc (a lángosok számától függően)",
                    PrepTimeInMinutes = 120, Difficulty = DifficultyLevel.Közepes, ImageUrl = "https://hungarytoday.hu/wp-content/uploads/2021/08/CSP_5259-scaled.jpg",
                    Tags = new List<string>()
                }
            };
            modelBuilder.Entity<Recipe>().HasData(recipes);

            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, RecipeId = 1, Name = "Darált marhahús", Quantity = 500, Unit = "g" },
                new Ingredient { Id = 2, RecipeId = 1, Name = "Spagetti tészta", Quantity = 400, Unit = "g" },
                new Ingredient { Id = 3, RecipeId = 1, Name = "Paradicsomszósz", Quantity = 500, Unit = "ml" },
                new Ingredient { Id = 4, RecipeId = 1, Name = "Vöröshagyma", Quantity = 1, Unit = "db" },
                new Ingredient { Id = 31, RecipeId = 2, Name = "Csirkemell filé", Quantity = 250, Unit = "g" },
                new Ingredient { Id = 32, RecipeId = 2, Name = "Római saláta", Quantity = 1, Unit = "fej" },
                new Ingredient { Id = 33, RecipeId = 2, Name = "Parmezán sajt", Quantity = 50, Unit = "g" },
                new Ingredient { Id = 8, RecipeId = 3, Name = "Lencse", Quantity = 300, Unit = "g" },
                new Ingredient { Id = 9, RecipeId = 3, Name = "Babérlevél", Quantity = 2, Unit = "db" },
                new Ingredient { Id = 10, RecipeId = 3, Name = "Liszt", Quantity = 2, Unit = "ek" },
                new Ingredient { Id = 11, RecipeId = 4, Name = "Csuszatészta", Quantity = 400, Unit = "g" },
                new Ingredient { Id = 12, RecipeId = 4, Name = "Tehéntúró", Quantity = 250, Unit = "g" },
                new Ingredient { Id = 13, RecipeId = 4, Name = "Füstölt szalonna", Quantity = 100, Unit = "g" },
                new Ingredient { Id = 14, RecipeId = 5, Name = "Lasagne tészta", Quantity = 12, Unit = "lap" },
                new Ingredient { Id = 15, RecipeId = 5, Name = "Darált hús", Quantity = 500, Unit = "g" },
                new Ingredient { Id = 16, RecipeId = 5, Name = "Besamel mártás", Quantity = 500, Unit = "ml" },
                new Ingredient { Id = 17, RecipeId = 6, Name = "Krumpli", Quantity = 1, Unit = "kg" },
                new Ingredient { Id = 18, RecipeId = 6, Name = "Füstölt kolbász", Quantity = 200, Unit = "g" },
                new Ingredient { Id = 19, RecipeId = 6, Name = "Tojás", Quantity = 6, Unit = "db" },
                new Ingredient { Id = 20, RecipeId = 6, Name = "Tejföl", Quantity = 450, Unit = "g" },
                new Ingredient { Id = 21, RecipeId = 7, Name = "Savanyú káposzta", Quantity = 1, Unit = "kg" },
                new Ingredient { Id = 22, RecipeId = 7, Name = "Darált sertéshús", Quantity = 500, Unit = "g" },
                new Ingredient { Id = 23, RecipeId = 7, Name = "Rizs", Quantity = 100, Unit = "g" },
                new Ingredient { Id = 24, RecipeId = 8, Name = "Paella rizs", Quantity = 300, Unit = "g" },
                new Ingredient { Id = 25, RecipeId = 8, Name = "Tenger gyümölcsei mix", Quantity = 400, Unit = "g" },
                new Ingredient { Id = 26, RecipeId = 8, Name = "Sáfrány", Quantity = 1, Unit = "csipet" },
                new Ingredient { Id = 27, RecipeId = 9, Name = "Leveles tészta", Quantity = 1, Unit = "csomag" },
                new Ingredient { Id = 28, RecipeId = 9, Name = "Krumpli", Quantity = 2, Unit = "db" },
                new Ingredient { Id = 29, RecipeId = 9, Name = "Zöldborsó", Quantity = 100, Unit = "g" },
                new Ingredient { Id = 30, RecipeId = 9, Name = "Garam masala fűszerkeverék", Quantity = 1, Unit = "tk" },
                new Ingredient { Id = 35, RecipeId = 11, Name = "Sertéslapocka", Quantity = 1, Unit = "kg" },
                new Ingredient { Id = 36, RecipeId = 11, Name = "Sertészsír", Quantity = 4, Unit = "evőkanál" },
                new Ingredient { Id = 37, RecipeId = 11, Name = "Vöröshagyma", Quantity = 3, Unit = "fej" },
                new Ingredient { Id = 38, RecipeId = 11, Name = "Paradicsom", Quantity = 1, Unit = "db" },
                new Ingredient { Id = 39, RecipeId = 11, Name = "Finomliszt", Quantity = 500, Unit = "g" },
                new Ingredient { Id = 40, RecipeId = 11, Name = "Tojás", Quantity = 2, Unit = "db" },
                new Ingredient { Id = 41, RecipeId = 11, Name = "Olaj", Quantity = 1, Unit = "evőkanál" },
                new Ingredient { Id = 42, RecipeId = 12, Name = "Marha lábszár", Quantity = 80, Unit = "dkg" },
                new Ingredient { Id = 43, RecipeId = 12, Name = "Vöröshagyma", Quantity = 2, Unit = "fej" },
                new Ingredient { Id = 44, RecipeId = 12, Name = "Sertészsír", Quantity = 4, Unit = "evőkanál" },
                new Ingredient { Id = 45, RecipeId = 12, Name = "Fokhagyma", Quantity = 2, Unit = "gerezd" },
                new Ingredient { Id = 46, RecipeId = 12, Name = "Babérlevél", Quantity = 3, Unit = "levél" },
                new Ingredient { Id = 47, RecipeId = 12, Name = "Sárgarépa", Quantity = 4, Unit = "szál" },
                new Ingredient { Id = 48, RecipeId = 12, Name = "Petrezselyemgyökér", Quantity = 2, Unit = "szál" },
                new Ingredient { Id = 49, RecipeId = 12, Name = "Petrezselyem", Quantity = 1, Unit = "köteg" },
                new Ingredient { Id = 50, RecipeId = 12, Name = "Burgonya", Quantity = 0.5m, Unit = "kg" },
                new Ingredient { Id = 51, RecipeId = 12, Name = "Paradicsom", Quantity = 1, Unit = "db" },
                new Ingredient { Id = 52, RecipeId = 13, Name = "Finomliszt", Quantity = 50, Unit = "dkg" },
                new Ingredient { Id = 53, RecipeId = 13, Name = "Élesztő", Quantity = 2.5m, Unit = "dkg" },
                new Ingredient { Id = 54, RecipeId = 13, Name = "Só", Quantity = 1, Unit = "teáskanál" },
                new Ingredient { Id = 55, RecipeId = 13, Name = "Cukor", Quantity = 1, Unit = "teáskanál" },
                new Ingredient { Id = 56, RecipeId = 13, Name = "Olaj", Quantity = 1, Unit = "l" },
                new Ingredient { Id = 57, RecipeId = 13, Name = "Tejföl", Quantity = 400, Unit = "g" },
                new Ingredient { Id = 58, RecipeId = 13, Name = "Sajt", Quantity = 250, Unit = "g" },
                new Ingredient { Id = 59, RecipeId = 13, Name = "Fokhagyma", Quantity = 1, Unit = "gerezd" }
            );
        }
    }
}