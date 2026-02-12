using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class Service
    {
        [Key]
        [Column("Servicio_ID")]
        public int ServiceID { get; set; }
        [Required]
        [Column("Nombre")]
        public string ServiceName { get; set; } = string.Empty;
        [Column("Descripcion")]
        public string Description { get; set; }= string.Empty;
        [Required]
        [Column("PrecioBase")]
        public decimal Price { get; set; }
        [Required]
        [Column("TiempoEstimado")]
        public string EstimatedTime { get; set; }= string.Empty;
    }
}
