using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Enums;
using Web_Drones_Proyect.Models;
using Web_Drones_Proyect.Services;

namespace Web_Drones_Proyect.Components.Pages.Drones
{
    // Componente que muestra la lista de drones en la página principal
    public partial class Index
    {
        [Inject] private CartService CartService { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        // Contexto de base de datos
        [Inject] private ApplicationDbContext _context { get; set; } = default!;

        // Servicio para mostrar diálogos
        [Inject] private IDialogService DialogService { get; set; } = default!;

        // Navegación entre páginas
        [Inject] private NavigationManager Nav { get; set; } = default!;

        // Lista de drones mostrados en la vista
        private List<Drone> _drones = new();

        // Carga los drones al iniciar el componente
        protected override async Task OnInitializedAsync()
        {
            _drones = await _context.Drones
                .Include(d => d.Images)
                .ToListAsync();
        }

        // Abre el diálogo para editar un dron
        private async Task EditDroneAsync(Drone drone)
        {
            var parameters = new DialogParameters
            {
                { "DroneToEdit", drone }
            };

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Large,
                FullWidth = true
            };

            var dialog = await DialogService.ShowAsync<AddDrone>("Editar Dron", parameters);
            var result = await dialog.Result;

            // Si se guardaron cambios, recarga la lista
            if (!result.Canceled)
            {
                _drones = await _context.Drones.Include(d => d.Images).ToListAsync();
                StateHasChanged();
            }
        }

        // Elimina un dron y sus imágenes del sistema
        private async Task DeleteDroneAsync(int droneId)
        {
            var drone = await _context.Drones
                .Include(d => d.Images)
                .FirstOrDefaultAsync(d => d.DronID == droneId);

            if (drone == null)
            {
                Snackbar.Add("El dron no existe o ya fue eliminado", Severity.Warning);
                return;
            }

            // 🔥 1. Eliminar items del carrito relacionados al dron
            var cartItems = _context.CartItems
                .Where(c => c.DronID == droneId);

            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            // 🔥 2. Eliminar imágenes físicas del servidor
            foreach (var img in drone.Images)
            {
                if (string.IsNullOrWhiteSpace(img.UrlImageDron))
                    continue;

                var filePath = Path.Combine(
                    Environment.CurrentDirectory,
                    "wwwroot",
                    "images",
                    "drones",
                    Path.GetFileName(img.UrlImageDron)
                );

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            // 🔥 3. Eliminar el dron
            _context.Drones.Remove(drone);
            await _context.SaveChangesAsync();

            // 🔄 4. Recargar lista
            _drones = await _context.Drones
                .Include(d => d.Images)
                .ToListAsync();

            StateHasChanged();

            // ✅ NOTIFICACIÓN DE ÉXITO
            Snackbar.Add("Dron eliminado correctamente", Severity.Success);
        }

        // Agrega un dron al carrito del usuario autenticado y muestra un mensaje según el resultado
        private async Task AddToCartAsync(Drone drone)
        {
            var auth = await AuthStateProvider.GetAuthenticationStateAsync();
            var userId = int.Parse(auth.User
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var result = await CartService.AddItemAsync(
                userId,
                drone.DronID,
                false,
                1
            );

            if (result == AddToCartResult.Success)
            {
                Snackbar.Add("Producto añadido al carrito", Severity.Success);
            }
            else
            {
                Snackbar.Add("Producto no disponible", Severity.Warning);
            }
        }

    }
}