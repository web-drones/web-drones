using Microsoft.AspNetCore.Components;
using MudBlazor;
using Web_Drones_Proyect.Components.Pages;

namespace Web_Drones_Proyect.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] private IDialogService DialogService { get; set; } = default!;

        private async Task OpenAddDialog()
        {
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Medium
            };

            await DialogService.ShowAsync<AddDrone>("Agregar Drone", options);
        }
    }
}