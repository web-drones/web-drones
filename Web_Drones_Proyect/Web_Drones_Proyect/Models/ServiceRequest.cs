using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Web_Drones_Proyect.Models
{
    public class ServiceRequest
    {
        [Key]
        [Column("SolicitudServicio_ID")]
        public int ServiceRequestID { get; set; }
        [Required]
        [Column("Usuario_ID")]
        public int UserID { get; set; }
        [Required]
        [Column("Servicio_ID")]
        public int ServiceID { get; set; }
        [Column("FechaSolicitud")]
        public DateTime ApplicationDate {  get; set; }
        [Required]
        [Column("Estado")]
        public string State { get; set; } = string.Empty;

        [Column("Observaciones")]
        public string Observations { get; set; } = string.Empty;

        public User User { get; set; } = null!;
        public Service Service { get; set; } = null!;

    }
}
