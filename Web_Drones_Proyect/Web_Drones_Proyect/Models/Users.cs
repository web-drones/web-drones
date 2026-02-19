using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class User
    {
        [Key]
        [Column("Usuario_ID")]
        public int UserID { get; set; }

        [Required]
        [Column("Nombre")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("Rol_ID")]
        public int RolID { get; set; }

        [Column("Activo")]
        public bool Active { get; set; }

        [Column("FechaRegistro")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [ForeignKey("RolID")]
        public Role Role { get; set; } = null!;
    }
}
