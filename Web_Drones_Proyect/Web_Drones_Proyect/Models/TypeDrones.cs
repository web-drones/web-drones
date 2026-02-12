using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class TypeDrones
    {
        [Key]
        [Column("TipoDron_ID")]
        public int TypeDroneID { get; set; }

        [Required]
        [Column("Nombre")]
        public string Name { get; set; } = string.Empty;
    }
}
