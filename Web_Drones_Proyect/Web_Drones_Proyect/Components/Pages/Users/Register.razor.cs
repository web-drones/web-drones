//using Microsoft.AspNetCore.Components;
//using MudBlazor;
//using System.Security.Cryptography;
//using System.Text;
//using Web_Drones_Proyect.Data.Repositories;
//using Web_Drones_Proyect.Models;

//namespace Web_Drones_Proyect.Components.Pages.Users
//{
//    public partial class Register
//    {
//        private MudForm _form = default!;
//        private User _user = new();
//        private string _password = string.Empty;
//        private string _confirmPassword = string.Empty;
//        private string? _errorMessage;

//        [Inject] private IRepository<User> UserRepository { get; set; } = default!;
//        [Inject] private NavigationManager Navigation { get; set; } = default!;

//        private async Task RegisterAsync()
//        {
//            if (_form != null)
//                await _form.Validate();

//            _errorMessage = string.Empty;

//            if (!_form.IsValid)
//                return;

//            if (_password != _confirmPassword)
//            {
//                _errorMessage = "Las contraseñas no coinciden.";
//                return;
//            }

//            // Verificar si ya existe el correo
//            var existingUser = (await UserRepository.FindAsync(u => u.Email == _user.Email)).FirstOrDefault();
//            if (existingUser != null)
//            {
//                _errorMessage = "Este correo ya está registrado.";
//                return;
//            }

//            // Asignar valores correctos
//            _user.Password = HashPassword(_password);
//            _user.RolID = 2; // Usuario normal
//            _user.Active = true;
//            _user.RegistrationDate = DateTime.Now;

//            await UserRepository.AddAsync(_user);
//            await UserRepository.SaveChangesAsync();

//            Navigation.NavigateTo("/login");
//        }

//        private string HashPassword(string password)
//        {
//            using var sha256 = SHA256.Create();
//            var bytes = Encoding.UTF8.GetBytes(password);
//            var hash = sha256.ComputeHash(bytes);
//            return Convert.ToBase64String(hash);
//        }
//    }
//}