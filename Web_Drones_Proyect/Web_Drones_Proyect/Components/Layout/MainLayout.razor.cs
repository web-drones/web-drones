using Microsoft.AspNetCore.Components;
using MudBlazor;
using Web_Drones_Proyect.Components.Pages.Drones;

namespace Web_Drones_Proyect.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private async Task OpenAddDialog()
        {
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Medium
            };

            var dialog = await DialogService.ShowAsync<AddDrone>("Agregar Drone", options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            }
        }
    }
}