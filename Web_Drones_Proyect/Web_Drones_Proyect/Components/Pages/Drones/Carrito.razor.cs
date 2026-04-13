using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Web_Drones_Proyect.Models;
using Web_Drones_Proyect.Services;

namespace Web_Drones_Proyect.Components.Pages.Drones
{
    // Lógica del carrito de compras del usuario
    public partial class Carrito
    {
        // Servicio para manejar operaciones del carrito
        [Inject] private CartService CartService { get; set; } = default!;

        // Proporciona información del usuario autenticado
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        // Muestra mensajes emergentes al usuario
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        // Maneja diálogos de confirmación
        [Inject] private IDialogService DialogService { get; set; } = default!;


        // Lista de productos del carrito
        private List<CartItem> _items = new();

        private UserEntity _cliente = new();
        private CartItem? _rentItem;
        private DateTime? _rentStart;
        private DateTime? _rentEnd;
        private int _rentQuantity = 1;

        // Se ejecuta al iniciar el componente
        protected override async Task OnInitializedAsync()
        {
            // Obtiene el estado de autenticación
            var auth = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = auth.User;

            // Sale si el usuario no está autenticado
            if (!user.Identity!.IsAuthenticated)
                return;

            // Obtiene el ID del usuario autenticado
            var userId = int.Parse(
                user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
            );

            // Carga los productos del carrito
            _items = await CartService.GetItemsAsync(userId);
        }

        private async Task LoadClienteAsync()
        {
            var auth = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = auth.User;

            if (!user.Identity!.IsAuthenticated)
                return;

            _cliente = new UserEntity
            {
                UserName = user.Identity.Name ?? "",
                Email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? ""
            };
        }

        // Elimina un producto del carrito
        private async Task Remove(int cartItemId)
        {
            // Elimina el producto del sistema
            await CartService.RemoveItemAsync(cartItemId);

            // Elimina el producto de la lista local
            _items.RemoveAll(i => i.CartItemID == cartItemId);

            // Notifica al usuario
            Snackbar.Add("Producto eliminado del carrito", Severity.Info);
        }

        // ===== TOTALES =====

        // Total a pagar de productos seleccionados
        private decimal TotalSeleccionado =>
            _items.Where(i => i.Selected)
                  .Sum(i => i.SubTotal);

        // Total de unidades en el carrito
        private int TotalProductos =>
            _items.Sum(i => i.Quantity);

        // Cantidad de productos seleccionados
        private int ProductosSeleccionados =>
            _items.Count(i => i.Selected);

        private bool EsVenta(CartItem item)
        {
            return item.Drone?.PriceSale.HasValue == true;
        }

        private bool EsRenta(CartItem item)
        {
            return item.Drone?.PriceRent.HasValue == true;
        }

        // ===== PAGOS =====

        // Paga solo los productos seleccionados
        private async void PagarSeleccionados()
        {
            var itemsSeleccionados = _items
                .Where(i => i.Selected)
                .ToList();

            if (!itemsSeleccionados.Any())
            {
                Snackbar.Add("⚠️ Primero debe seleccionar algún artículo", Severity.Warning);
                return;
            }

            await LoadClienteAsync();

            DialogService.Show<CartConfirmDialog>(
                "Confirmar pedido",
                new DialogParameters
                {
                    ["Items"] = itemsSeleccionados,
                    ["Total"] = itemsSeleccionados.Sum(i => i.SubTotal),
                    ["Cliente"] = _cliente
                });
        }

        // Paga todos los productos del carrito
        private async void PagarTodo()
        {
            await LoadClienteAsync();

            DialogService.Show<CartConfirmDialog>(
                "Confirmar pedido",
                new DialogParameters
                {
                    ["Items"] = _items,
                    ["Total"] = _items.Sum(i => i.SubTotal),
                    ["Cliente"] = _cliente
                });
        }

        private void OpenRentModal(CartItem item)
        {
            if (!item.Drone.PriceRent.HasValue)
            {
                Snackbar.Add("Este dron no tiene opción de renta", Severity.Warning);
                return;
            }

            _rentItem = item;
            _rentStart = DateTime.Today;
            _rentEnd = DateTime.Today.AddDays(1);
            _rentQuantity = 1;

            DialogService.Show<RentDialog>(
                "Rentar dron",
                new DialogParameters
                {
                    ["Item"] = item,
                    ["StartDate"] = _rentStart,
                    ["EndDate"] = _rentEnd,
                    ["Quantity"] = _rentQuantity
                });
        }

    }
}