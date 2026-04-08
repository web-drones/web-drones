using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class DronRent
    {
        [Key]
        [Column("Renta_ID")]
        public int RentID { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Column("Usuario_ID")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "El dron es obligatorio")]
        [Column("Dron_ID")]
        public int DronID { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Column("FechaInicio")]
        public DateTime StartingDate { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [Column("FechaFin")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "El estado de la renta es obligatorio")]
        [Column("Estado")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "El total es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser 0 o mayor")]
        [Column("Total")]
        public decimal Price { get; set; }

        // =========================
        // RELACIONES
        // =========================

        public User User { get; set; } = null!;
        public Drone Drone { get; set; } = null!;
    }
}