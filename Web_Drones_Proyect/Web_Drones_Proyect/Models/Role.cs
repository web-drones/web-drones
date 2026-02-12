using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class Role
    {
        [Key]
        [Column("Rol_ID")]
        public int RoleID { get; set; }
        [Required]
        [Column("Nombre")]
        public string Name { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
