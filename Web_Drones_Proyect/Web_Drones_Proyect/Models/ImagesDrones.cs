using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class ImagesDrones
    {
        [Key]
        [Column("DronImagen_ID")]
        public int DronImageID { get; set; }
        [Required]
        [Column("Dron_ID")]
        public int DronID { get; set; }
        [Required]
        [Column("UrlImagen")]
        public string UrlImageDron { get; set; } = string.Empty;
        public Drone Drone { get; set; } = null!;

    }
}
