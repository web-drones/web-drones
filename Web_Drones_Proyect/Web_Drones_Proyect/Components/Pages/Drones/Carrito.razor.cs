using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Web_Drones_Proyect.Models;
using Web_Drones_Proyect.Services;

namespace Web_Drones_Proyect.Components.Pages.Drones
{
    public partial class Carrito
    {
        [Inject] private CartService CartService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        private List<CartItem> _items = new();

        protected override async Task OnInitializedAsync()
        {
            var auth = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = auth.User;

            if (!user.Identity!.IsAuthenticated)
                return;

            var userId = int.Parse(
                user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
            );

            _items = await CartService.GetItemsAsync(userId);
        }

        private async Task Remove(int cartItemId)
        {
            await CartService.RemoveItemAsync(cartItemId);

            _items.RemoveAll(i => i.CartItemID == cartItemId);

            Snackbar.Add("Producto eliminado del carrito", Severity.Info);
        }
    }
}