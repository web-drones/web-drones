using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Web_Drones_Proyect.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        public AuthService(ApplicationDbContext context, AuthenticationStateProvider authStateProvider)
        {
            _context = context;
            _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _context.Usuarios
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.Active);

            if (user == null) return false;

            var hash = HashPassword(password);

            Console.WriteLine("HASH INGRESADO: " + hash);
            Console.WriteLine("HASH BD: " + user.PasswordHash);

            if (user.PasswordHash != hash) return false;

            _authStateProvider.MarkUserAsAuthenticated(user);
            return true;
        }


        public Task LogoutAsync()
        {
            _authStateProvider.MarkUserAsLoggedOut();
            return Task.CompletedTask;
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
