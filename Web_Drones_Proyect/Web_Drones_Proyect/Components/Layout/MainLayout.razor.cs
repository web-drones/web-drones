using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Web_Drones_Proyect.Components.Pages.Drones;
using Web_Drones_Proyect.Services;


namespace Web_Drones_Proyect.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthService AuthService { get; set; } = default!;
        [CascadingParameter] private Task<AuthenticationState> AuthStateTask { get; set; } = default!;

        private string userName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateTask;
            var user = authState.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                userName = user.Identity.Name ?? string.Empty;
            }
        }

        private async Task OpenAddDialog()
        {
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var dialog = await DialogService.ShowAsync<AddDrone>("Agregar Drone", options);
            var result = await dialog.Result;

            if (!result.Canceled)
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
        }

        private async Task LogoutAsync()
        {
            await AuthService.LogoutAsync();
            Navigation.NavigateTo("/", true);      
        }

    }
}
