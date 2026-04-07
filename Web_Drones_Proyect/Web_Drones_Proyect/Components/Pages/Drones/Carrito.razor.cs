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

        // ===== PAGOS =====

        // Paga solo los productos seleccionados
        private void PagarSeleccionados()
        {
            var itemsSeleccionados = _items
                .Where(i => i.Selected)
                .ToList();

            // ❗ VALIDACIÓN CLAVE
            if (!itemsSeleccionados.Any())
            {
                Snackbar.Add(
                    "⚠️ Primero debe seleccionar algún artículo",
                    Severity.Warning
                );
                return;
            }

            DialogService.Show<CartConfirmDialog>(
                "Confirmar pedido",
                new DialogParameters
                {
                    ["Items"] = itemsSeleccionados,
                    ["Total"] = itemsSeleccionados.Sum(i => i.SubTotal)
                });
        }

        // Paga todos los productos del carrito
        private void PagarTodo()
        {
            // Abre el diálogo de confirmación
            DialogService.Show<CartConfirmDialog>(
                "Confirmar pedido",
                new DialogParameters
                {
                    ["Items"] = _items,
                    ["Total"] = _items.Sum(i => i.SubTotal)
                });
        }

    }
}