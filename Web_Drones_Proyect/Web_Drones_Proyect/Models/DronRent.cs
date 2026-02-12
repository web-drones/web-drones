using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class DronRent
    {
        [Key]
        [Column("Renta_ID")]
        public int RentID { get; set; }
        [Required]
        [Column("Usuario_ID")]
        public int UserID { get; set; }
        [Required]
        [Column("Dron_ID")]
        public int DronID { get; set; }
        [Required]
        [Column("FechaInicio")]
        public DateTime StartingDate { get; set; }

        [Required]
        [Column("FechaFin")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column("Estado")]
        public string State { get; set; } = string.Empty; 
        [Required]
        [Column("Total")]
        public decimal Price { get; set; }

        public User User { get; set; } = null!;
        public Drone Drone { get; set; } = null!;

    }
}
