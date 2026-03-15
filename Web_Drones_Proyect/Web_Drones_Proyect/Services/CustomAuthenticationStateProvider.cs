using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private ClaimsPrincipal _currentUser = null!;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_currentUser != null)
                return Task.FromResult(new AuthenticationState(_currentUser));

            return Task.FromResult(new AuthenticationState(_anonymous));
        }

        public void MarkUserAsAuthenticated(User user)
        {
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
    new Claim(ClaimTypes.Name, user.UserName),
    new Claim(ClaimTypes.Email, user.Email),
    // Asegúrate de usar ClaimTypes.Role
    new Claim(ClaimTypes.Role, user.Role?.Name ?? "Cliente")
};

            var identity = new ClaimsIdentity(
                claims,
                "apiauth_type",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType // <- Esto es importante
            );

            _currentUser = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public void MarkUserAsLoggedOut()
        {
            _currentUser = _anonymous;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }
    }
}
