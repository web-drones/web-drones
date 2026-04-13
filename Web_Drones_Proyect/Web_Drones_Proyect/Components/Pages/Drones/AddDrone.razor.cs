using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using MudBlazor;
using System.Linq;
using Web_Drones_Proyect.Data.Repositories;
using Web_Drones_Proyect.Enums;
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
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

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
                    State = DroneToEdit.State?.Trim(), // 👈 IMPORTANTE
                    Stock = DroneToEdit.Stock,
                    Autonomy = DroneToEdit.Autonomy,
                    Scope = DroneToEdit.Scope,
                    Camera = DroneToEdit.Camera,
                    Description = DroneToEdit.Description,
                    Images = DroneToEdit.Images
                };

                _imagePath = DroneToEdit.Images.FirstOrDefault()?.UrlImageDron;
            }

            await InvokeAsync(StateHasChanged);
        }

        // Guarda el dron (crear o actualizar)
        private bool IsValidMoney(decimal? value)
        {
            if (!value.HasValue)
                return false;

            return value.Value >= 0 && value.Value <= 999999999.99m;
        }

        private async Task Save()
        {
            await _form!.Validate();

            if (!_form.IsValid)
                return;

            // Validación de imagen
            if (string.IsNullOrWhiteSpace(_imagePath))
            {
                Snackbar.Add("La imagen del dron es obligatoria", Severity.Error);
                return;
            }

            // Validación de precios (evita overflow SQL)
            if (!IsValidMoney(_drone.PriceSale))
            {
                Snackbar.Add("El precio de venta es demasiado grande (máx: 999,999,999.99)", Severity.Error);
                return;
            }

            if (!IsValidMoney(_drone.PriceRent))
            {
                Snackbar.Add("El precio de renta es demasiado grande (máx: 999,999,999.99)", Severity.Error);
                return;
            }

            // Valor por defecto
            if (string.IsNullOrWhiteSpace(_drone.Camera))
                _drone.Camera = "Sin cámara";

            // 🔥 REGLA CLAVE: recalcular Availability SIEMPRE
            _drone.Availability =
                _drone.PriceSale > 0 && _drone.PriceRent > 0
                    ? DroneAvailability.Both
                    : _drone.PriceSale > 0
                        ? DroneAvailability.Sale
                        : _drone.PriceRent > 0
                            ? DroneAvailability.Rent
                            : 0;

            try
            {
                if (DroneToEdit == null)
                {
                    // Crear
                    await DroneRepository.AddAsync(_drone);
                    await DroneRepository.SaveChangesAsync();

                    var image = new ImagesDrones
                    {
                        DronID = _drone.DronID,
                        UrlImageDron = _imagePath
                    };

                    await ImageRepository.AddAsync(image);
                    await ImageRepository.SaveChangesAsync();

                    Snackbar.Add("Dron creado correctamente", Severity.Success);
                }
                else
                {
                    // Editar
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

                    // 🔥 REGLA CLAVE TAMBIÉN EN EDITAR
                    DroneToEdit.Availability =
                        DroneToEdit.PriceSale > 0 && DroneToEdit.PriceRent > 0
                            ? DroneAvailability.Both
                            : DroneToEdit.PriceSale > 0
                                ? DroneAvailability.Sale
                                : DroneToEdit.PriceRent > 0
                                    ? DroneAvailability.Rent
                                    : 0;

                    DroneRepository.Update(DroneToEdit);
                    await DroneRepository.SaveChangesAsync();

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

                    Snackbar.Add("Dron actualizado correctamente", Severity.Success);
                }

                MudDialog.Close(DialogResult.Ok(true));
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error al guardar: {ex.Message}", Severity.Error);
            }
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