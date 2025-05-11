using ES2Real.Components;
using Microsoft.EntityFrameworkCore;
using Blazored.LocalStorage;
using ES2Real.Data;
using ES2Real.Interfaces;
using ES2Real.TipoUsuarioHandlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.OpenApi.Models; // Adicionado para o Swagger

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Database (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://localhost:44343/"); // Altere para o endereço correto da sua API
});



builder.Services.AddAuthorization();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<utilizadorService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44343/") });
builder.Services.AddScoped<utilizadorService>();

builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddSingleton<UserSessionService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ITipoUsuarioHandler, ParticipanteHandler>();
builder.Services.AddScoped<ITipoUsuarioHandler, OrganizadorHandler>();



// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sandbox API",
        Version = "v1",
        Description = "API para a aplicação Blazor Sandbox"
    });
});

var options = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Conectando ao banco: {options}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// 🔹 Ativando o Swagger (após o `if`)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
        c.RoutePrefix = "swagger"; // URL será "/swagger"
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Enable Authentication & Authorization before mapping controllers
app.UseAuthentication();
app.UseAuthorization();

// Map API Controllers
app.MapControllers();

// Map Blazor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map API Endpoints
app.MapControllers();
app.Run();
