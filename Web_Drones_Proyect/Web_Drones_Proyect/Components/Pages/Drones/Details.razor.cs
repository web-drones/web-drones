using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Components.Pages.Drones
{
    // Lógica del componente que muestra los detalles de un dron
    public partial class Details
    {
        // Id del dron recibido desde la URL
        [Parameter] public int Id { get; set; }

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
    }
}