using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using MudBlazor;
using System.Linq;
using Web_Drones_Proyect.Data.Repositories;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Components.Pages.Drones
{
    public partial class AddDrone
    {
        [Inject] private IWebHostEnvironment Env { get; set; } = default!;
        [Inject] private IRepository<Drone> DroneRepository { get; set; } = default!;
        [Inject] private IRepository<Category> CategoryRepository { get; set; } = default!;
        [Inject] private IRepository<TypeDrones> TypeRepository { get; set; } = default!;
        [Inject] private IRepository<ImagesDrones> ImageRepository { get; set; } = default!;

        [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public Drone? DroneToEdit { get; set; }

        private MudForm? _form;
        private Drone _drone = new();

        private IEnumerable<Category> _categories = new List<Category>();
        private IEnumerable<TypeDrones> _types = new List<TypeDrones>();

        private string? _imagePath;

        protected override async Task OnInitializedAsync()
        {
            _categories = await CategoryRepository.GetAllAsync();
            _types = await TypeRepository.GetAllAsync();

            if (DroneToEdit != null)
            {
              
                _drone = new Drone
                {
                    DronID = DroneToEdit.DronID,
                    Name = DroneToEdit.Name,
                    CategoryID = DroneToEdit.CategoryID,
                    TypeDroneID = DroneToEdit.TypeDroneID,
                    PriceSale = DroneToEdit.PriceSale,
                    PriceRent = DroneToEdit.PriceRent,
                    State = DroneToEdit.State,
                    Stock = DroneToEdit.Stock,
                    Autonomy = DroneToEdit.Autonomy,
                    Scope = DroneToEdit.Scope,
                    Camera = DroneToEdit.Camera,
                    Description = DroneToEdit.Description,
                    Images = DroneToEdit.Images
                };

                
                _imagePath = DroneToEdit.Images.FirstOrDefault()?.UrlImageDron;
            }
            else
            {
                _drone.State = "Disponible";

                if (_categories.Any())
                    _drone.CategoryID = _categories.First().CategoryID;

                if (_types.Any())
                    _drone.TypeDroneID = _types.First().TypeDroneID;

            }
        }

        private async Task Save()
        {
            await _form!.Validate();
            if (!_form.IsValid) return;

            if (DroneToEdit == null)
            {
                Console.WriteLine($"CategoryID: {_drone.CategoryID}");
                Console.WriteLine($"TypeDroneID: {_drone.TypeDroneID}");

                await DroneRepository.AddAsync(_drone);
                await DroneRepository.SaveChangesAsync();

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
            }
            else
            {
                DroneToEdit.Name = _drone.Name;
                DroneToEdit.CategoryID = _drone.CategoryID;
                DroneToEdit.TypeDroneID = _drone.TypeDroneID;
                DroneToEdit.PriceSale = _drone.PriceSale;
                DroneToEdit.PriceRent = _drone.PriceRent;
                DroneToEdit.State = _drone.State;
                DroneToEdit.Stock = _drone.Stock;
                DroneToEdit.Autonomy = _drone.Autonomy;
                DroneToEdit.Scope = _drone.Scope;
                DroneToEdit.Camera = _drone.Camera;
                DroneToEdit.Description = _drone.Description;

                DroneRepository.Update(DroneToEdit);
                await DroneRepository.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(_imagePath))
                {
                    var existingImage = DroneToEdit.Images.FirstOrDefault();

                    if (existingImage != null)
                    {
                        existingImage.UrlImageDron = _imagePath;
                        ImageRepository.Update(existingImage);
                    }
                    else
                    {
                        var image = new ImagesDrones
                        {
                            DronID = DroneToEdit.DronID,
                            UrlImageDron = _imagePath
                        };

                        await ImageRepository.AddAsync(image);
                    }

                    await ImageRepository.SaveChangesAsync();
                }
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

            _imagePath = $"/images/drones/{fileName}";
        }
    }

}