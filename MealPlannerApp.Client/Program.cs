using MealPlannerApp.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- Szolgáltatások regisztrálása ---

// 1. MudBlazor szolgáltatások
builder.Services.AddMudServices();

// 2. LocalStorage (a tokenek tárolásához a böngészõben)
builder.Services.AddBlazoredLocalStorage();

// 3. Azonosítási szolgáltatások bekapcsolása
builder.Services.AddAuthorizationCore();

// 4. Saját AuthenticationStateProvider regisztrálása
// Itt mondjuk meg a Blazornak, hogy a mi osztályunkat használja az állapot figyelésére
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<ApiAuthenticationStateProvider>());

// 5. HttpClient beállítása
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();