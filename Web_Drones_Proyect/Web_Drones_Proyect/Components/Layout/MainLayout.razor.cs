using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Web_Drones_Proyect.Components.Pages.Drones;
using Web_Drones_Proyect.Services;

namespace Web_Drones_Proyect.Components.Layout
{
    // Lógica asociada al componente MainLayout
    public partial class MainLayout
    {
        // Servicio de MudBlazor para abrir diálogos modales
        [Inject] private IDialogService DialogService { get; set; } = default!;

        // Servicio de navegación entre páginas
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        // Servicio personalizado para manejo de autenticación
        [Inject] private AuthService AuthService { get; set; } = default!;

        // Estado de autenticación disponible para el layout
        [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = default!;

        // Nombre del usuario autenticado
        private string userName = string.Empty;

        // Se ejecuta al inicializar el componente
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateTask;
            var user = authState.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                userName = user.Identity.Name ?? string.Empty;
            }
        }

        // Abre el modal para agregar un nuevo dron
        private async Task OpenAddDialog()
        {
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var dialog = await DialogService.ShowAsync<AddDrone>("Agregar Drone", options);
            var result = await dialog.Result;

            // Recarga la página si se agregó un dron correctamente
            if (!result.Canceled)
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
        }

        // Cierra la sesión del usuario actual
        private async Task LogoutAsync()
        {
            await AuthService.LogoutAsync();
            Navigation.NavigateTo("/", true);
        }
    }
}