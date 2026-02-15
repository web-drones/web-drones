using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using MudBlazor;
using System.Linq;
using Web_Drones_Proyect.Data.Repositories;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Components.Pages
{
    public partial class AddDrone
    {
        [Inject] private IWebHostEnvironment Env { get; set; } = default!;
        [Inject] private IRepository<Drone> DroneRepository { get; set; } = default!;
        [Inject] private IRepository<Category> CategoryRepository { get; set; } = default!;
        [Inject] private IRepository<TypeDrones> TypeRepository { get; set; } = default!;
        [Inject] private IRepository<ImagesDrones> ImageRepository { get; set; } = default!;

        [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

        private MudForm? _form;
        private Drone _drone = new();

        private IEnumerable<Category> _categories = new List<Category>();
        private IEnumerable<TypeDrones> _types = new List<TypeDrones>();

        private string? _imagePath;

        protected override async Task OnInitializedAsync()
        {
            _categories = await CategoryRepository.GetAllAsync();
            _types = await TypeRepository.GetAllAsync();

            _drone.State = "Disponible";
        }

        private async Task Save()
        {
            await _form!.Validate();

            if (!_form.IsValid)
                return;

            // 1️⃣ Guardar el dron
            await DroneRepository.AddAsync(_drone);
            await DroneRepository.SaveChangesAsync();

            // 2️⃣ Si hay imagen, guardarla en la tabla DronImagenes
            if (!string.IsNullOrWhiteSpace(_imagePath))
            {
                var image = new ImagesDrones
                {
                    DronID = _drone.DronID,
                    UrlImageDron = _imagePath
                };

                await ImageRepository.AddAsync(image);
                await ImageRepository.SaveChangesAsync();
            }

            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task UploadImage(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file == null)
                return;

            var uploadsFolder = Path.Combine(Env.WebRootPath, "images", "drones");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.Name}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = File.Create(filePath);
            await file.OpenReadStream().CopyToAsync(stream);

            _imagePath = $"images/drones/{fileName}";
        }
    }

}