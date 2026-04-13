using System;

namespace Web_Drones_Proyect.Models
{
    public class UserEntity : BaseEntity
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RolID { get; set; } = 2; // 1 = Admin, 2 = Usuario normal
        public bool Active { get; set; } = true;
    }
}