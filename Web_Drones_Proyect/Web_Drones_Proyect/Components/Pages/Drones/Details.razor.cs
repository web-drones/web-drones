using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Components.Pages.Drones
{
    public partial class Details
    {
        [Parameter] public int Id { get; set; }

        [Inject] private ApplicationDbContext _context { get; set; } = default!;

        private Drone? _drone;
        private string? _selectedImage;

        protected override async Task OnInitializedAsync()
        {
            _drone = await _context.Drones
                .Include(d => d.Images)
                .Include(d => d.TypeDrone)
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.DronID == Id);

            if (_drone != null)
            {
                _selectedImage = _drone.Images.FirstOrDefault()?.UrlImageDron;
            }
        }

        private void SelectImage(string url)
        {
            _selectedImage = url;
        }

        private string GetThumbnailClass(string url)
        {
            return url == _selectedImage
            ? "thumbnail active-thumb"
            : "thumbnail";
        }
    }

}