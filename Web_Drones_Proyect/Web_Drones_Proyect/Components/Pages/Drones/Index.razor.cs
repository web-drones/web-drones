using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using Web_Drones_Proyect.Components.Pages.Drones;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Components.Pages.Drones
{
    public partial class Index
    {
        [Inject] private ApplicationDbContext _context { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private List<Drone> _drones = new();

        protected override async Task OnInitializedAsync()
        {
            _drones = await _context.Drones
                .Include(d => d.Images)
                .ToListAsync();
        }

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

            if (!result.Canceled)
            {
                _drones = await _context.Drones.Include(d => d.Images).ToListAsync();
                StateHasChanged();
            }
        }

        private async Task DeleteDroneAsync(int droneId)
        {
            var drone = await _context.Drones
                .Include(d => d.Images)
                .FirstOrDefaultAsync(d => d.DronID == droneId);

            if (drone != null)
            {
                
                foreach (var img in drone.Images)
                {
                    var filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "images", "drones", Path.GetFileName(img.UrlImageDron));
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }

                _context.Drones.Remove(drone);
                await _context.SaveChangesAsync();

                _drones = await _context.Drones.Include(d => d.Images).ToListAsync();
                StateHasChanged();
            }
        }
    }
}