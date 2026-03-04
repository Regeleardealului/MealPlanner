using MealPlannerApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json", optional: false, reloadOnChange: true);

// --- Adatbázis konfiguráció ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 1. Identity beállítása (Felhasználók és jelszavak kezelése) ---
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// --- 2. JWT konfiguráció (Biztonsági tokenek kezelése) ---
builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "MealPlannerApp",
        ValidAudience = "MealPlannerApp",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EzEgyNagyonHosszuEsBiztonsagosTitkosKulcs1234567890!"))
    };
});

builder.Services.AddControllers();

// Swagger/OpenAPI hozzáadása
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- HTTP request pipeline konfigurálása ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Blazor fájlok kiszolgálása
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// --- FONTOS: Hitelesítés és Jogosultságkezelés ---
// A UseAuthentication megmondja, ki a felhasználó, 
// a UseAuthorization pedig megmondja, mihez van joga.
app.UseAuthentication();
app.UseAuthorization();

// API Controllerek feltérképezése
app.MapControllers();

// Blazor kliens indítása
app.MapFallbackToFile("index.html");

app.Run();