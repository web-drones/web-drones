using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Web_Drones_Proyect.Components;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Data.Repositories;
using Web_Drones_Proyect.Services;

var builder = WebApplication.CreateBuilder(args);

// Obtiene la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configura Razor Components usando el modelo interactivo de servidor (.NET 8)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registra el DbContext para acceso a la base de datos con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configura autenticación basada en cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Página a la que se redirige cuando el usuario no está autenticado
        options.LoginPath = "/login";

        // Página mostrada cuando el usuario no tiene permisos
        options.AccessDeniedPath = "/";
    });

// Habilita el sistema de autorización
builder.Services.AddAuthorization();

// Registra los servicios de la librería de componentes MudBlazor
builder.Services.AddMudServices();

// Registro de repositorios genéricos y servicios de negocio
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<AuthService>();

// Permite acceder al HttpContext dentro de servicios
builder.Services.AddHttpContextAccessor();

// Proporciona el estado de autenticación a los componentes Blazor
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthorizationCore();

// Proveedor personalizado para manejar el estado de autenticación del usuario
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddScoped<AuthService>();

// Servicio de carrito de compras por usuario (Scoped):
// Mantiene los productos del carrito durante la sesión del usuario autenticado
builder.Services.AddScoped<CartService>();


var app = builder.Build();

// Configuración del pipeline de errores para entorno de producción
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Redirección automática a HTTPS
app.UseHttpsRedirection();

// Permite servir archivos estáticos (CSS, JS, imágenes, etc.)
app.UseStaticFiles();

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Protección contra ataques CSRF
app.UseAntiforgery();

// Mapea los componentes Razor como punto de entrada de la aplicación
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();