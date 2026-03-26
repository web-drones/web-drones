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
    // Lógica del componente que muestra los detalles de un dron
    public partial class Details
    {
        // Id del dron recibido desde la URL
        [Parameter] public int Id { get; set; }

        [Inject] private CartService CartService { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        // Contexto de base de datos
        [Inject] private ApplicationDbContext _context { get; set; } = default!;

        // Dron cargado desde la base de datos
        private Drone? _drone;

        // Imagen principal mostrada
        private string? _selectedImage;

        // Controla el estado de carga
        private bool _loading = true;

        // Carga el dron cuando el componente inicia
        protected override async Task OnInitializedAsync()
        {
            _drone = await _context.Drones
                .Include(d => d.Images)
                .Include(d => d.TypeDrone)
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.DronID == Id);

            // Selecciona la primera imagen disponible
            if (_drone != null)
            {
                _selectedImage = _drone.Images?.FirstOrDefault()?.UrlImageDron ?? "/images/no-image.png";
            }

            _loading = false;
        }

        private async Task AddToCartAsync()
        {
            if (_drone == null) return;

            var auth = await AuthStateProvider.GetAuthenticationStateAsync();
            var userId = int.Parse(
                auth.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
            );

            // Determinar si es renta o venta
            bool isRent;

            if (_drone.PriceSale.HasValue && !_drone.PriceRent.HasValue)
            {
                // Solo venta
                isRent = false;
            }
            else if (!_drone.PriceSale.HasValue && _drone.PriceRent.HasValue)
            {
                // Solo renta
                isRent = true;
            }
            else
            {
                // Ambos disponibles → por ahora venta por defecto
                isRent = false;
            }

            var result = await CartService.AddItemAsync(
                userId,
                _drone.DronID,
                isRent
            );

            // Mensajes según resultado
            switch (result)
            {
                case AddToCartResult.Success:
                    Snackbar.Add("Producto añadido al carrito", Severity.Success);
                    break;

                case AddToCartResult.OutOfStock:
                    Snackbar.Add("Producto no disponible", Severity.Warning);
                    break;

                case AddToCartResult.NotAvailable:
                    Snackbar.Add("Este producto no está disponible para esta operación", Severity.Warning);
                    break;
            }
        }
    }
}