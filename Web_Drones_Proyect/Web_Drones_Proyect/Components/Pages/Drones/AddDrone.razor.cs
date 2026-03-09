using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using MudBlazor;
using System.Linq;
using Web_Drones_Proyect.Data.Repositories;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Components.Pages.Drones
{
    // Lógica del diálogo para crear o editar drones
    public partial class AddDrone
    {
        // Entorno del servidor (para acceder a wwwroot)
        [Inject] private IWebHostEnvironment Env { get; set; } = default!;

        // Repositorios para acceso a datos
        [Inject] private IRepository<Drone> DroneRepository { get; set; } = default!;
        [Inject] private IRepository<Category> CategoryRepository { get; set; } = default!;
        [Inject] private IRepository<TypeDrones> TypeRepository { get; set; } = default!;
        [Inject] private IRepository<ImagesDrones> ImageRepository { get; set; } = default!;

        // Instancia del diálogo MudBlazor
        [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

        // Drone recibido cuando se está editando
        [Parameter] public Drone? DroneToEdit { get; set; }

        // Referencia al formulario
        private MudForm? _form;

        // Modelo del dron en edición
        private Drone _drone = new();

        // Listas para los selectores
        private IEnumerable<Category> _categories = new List<Category>();
        private IEnumerable<TypeDrones> _types = new List<TypeDrones>();

        // Ruta de la imagen seleccionada
        private string? _imagePath;

        // Inicializa datos necesarios del formulario
        protected override async Task OnInitializedAsync()
        {
            _categories = await CategoryRepository.GetAllAsync();
            _types = await TypeRepository.GetAllAsync();

            if (DroneToEdit != null)
            {
                // Copia los datos del dron a editar
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

                // Obtiene la imagen existente
                _imagePath = DroneToEdit.Images.FirstOrDefault()?.UrlImageDron;
            }
            else
            {
                // Valores por defecto al crear un nuevo dron
                _drone.State = "Disponible";

                if (_categories.Any())
                    _drone.CategoryID = _categories.First().CategoryID;

                if (_types.Any())
                    _drone.TypeDroneID = _types.First().TypeDroneID;
            }
        }

        // Guarda el dron (crear o actualizar)
        private async Task Save()
        {
            await _form!.Validate();
            if (!_form.IsValid) return;

            if (DroneToEdit == null)
            {
                // Crear nuevo dron
                await DroneRepository.AddAsync(_drone);
                await DroneRepository.SaveChangesAsync();

                // Guardar imagen asociada
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
                // Actualizar dron existente
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

                // Actualiza o crea la imagen del dron
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

            // Cierra el diálogo indicando éxito
            MudDialog.Close(DialogResult.Ok(true));
        }

        // Cancela el diálogo
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        // Maneja la carga de imagen del dron
        private async Task UploadImage(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file == null)
                return;

            // Carpeta donde se guardarán las imágenes
            var uploadsFolder = Path.Combine(Env.WebRootPath, "images", "drones");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Genera nombre único para el archivo
            var fileName = $"{Guid.NewGuid()}_{file.Name}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = File.Create(filePath);
            await file.OpenReadStream().CopyToAsync(stream);

            // Ruta pública de la imagen
            _imagePath = $"/images/drones/{fileName}";
        }
    }
}