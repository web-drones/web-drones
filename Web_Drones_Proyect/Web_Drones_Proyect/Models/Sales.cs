using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class Sale
    {
        [Key]
        [Column("Venta_ID")]
        public int SaleID { get; set; }
        [Required]
        [Column("Usuario_ID")]
        public int UserID { get; set; }
        [Column("FechaVenta")]
        public DateTime SaleDate { get; set; }
        [Required]
        public decimal Total { get; set; }

        public ICollection<DetailsSale> DetailsSales { get; set; } = new List<DetailsSale>();
    }
}
