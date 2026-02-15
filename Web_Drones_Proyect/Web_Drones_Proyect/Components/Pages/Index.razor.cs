using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Components.Pages
{
    public partial class Index
    {
        [Inject] private ApplicationDbContext _context { get; set; } = default!;

        private List<Drone> _drones = new();

        protected override async Task OnInitializedAsync()
        {
            // Cargamos los drones con sus imágenes (si existen)
            _drones = await _context.Drones
                .Include(d => d.Images)
                .ToListAsync();
        }
    }
}