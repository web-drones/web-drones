using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class DetailsSale
    {
        [Key]
        [Column("VentaDetalle_ID")]
        public int SaleDetailsID { get; set; }

        // FK a Venta
        [Column("Venta_ID")]
        public int SaleID { get; set; }
        public Sale Sale { get; set; } = null!;

        // FK a Drone
        [Column("Dron_ID")]
        public int DronID { get; set; }
        public Drone Drone { get; set; } = null!;

        [Column("Cantidad")]
        public int Amount { get; set; }

        [Column("PrecioUnitario")]
        public decimal UnitPrice { get; set; }
    }
}
