using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Web_Drones_Proyect.Models
{
    public class Drone
    {
        [Key]
        [Column("Dron_ID")]
        public int DronID { get; set; }

        [Required]
        [Column("Nombre")]
        public string Name { get; set; } = string.Empty;

        [Column("PrecioVenta")]
        public decimal? PriceSale { get; set; }

        [Column("PrecioRenta")]
        public decimal? PriceRent { get; set; }

        [Required]
        [Column("Estado")]
        public string State { get; set; } = string.Empty;

        [Required]
        public int Stock { get; set; }

        [Column("Autonomia")]
        public int Autonomy { get; set; }

        [Column("Alcance")]
        public int Scope { get; set; }

        [Column("Camara")]
        public string Camera { get; set; } = string.Empty;
        [Column("Descripcion")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // FK a Tipo de Dron
        [Column("TipoDron_ID")]
        public int TypeDroneID { get; set; }

        [ForeignKey("TypeDroneID")]
        public TypeDrones TypeDrone { get; set; } = null!;

        // FK a Categoria
        [Column("Categoria_ID")]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category Category { get; set; } = null!;

        // Relaciones
        public ICollection<DetailsSale> SaleDetails { get; set; } = new List<DetailsSale>();
        public ICollection<DronRent> Rents { get; set; } = new List<DronRent>();
        public ICollection<ImagesDrones> Images { get; set; } = new List<ImagesDrones>();
    }
}
