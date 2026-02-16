using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Web_Drones_Proyect.Components;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Obtener connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Servicios
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(connectionString));
builder.Services.AddMudServices();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddMudServices();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
