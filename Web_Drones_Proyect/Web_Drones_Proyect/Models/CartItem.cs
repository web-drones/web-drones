using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_Drones_Proyect.Enums;

namespace Web_Drones_Proyect.Models
{
    public class CartItem
    {
        [Key]
        [Column("CartItem_ID")]
        public int CartItemID { get; set; }

        [Required]
        [Column("Cart_ID")]
        public int CartID { get; set; }

        [Required]
        [Column("Dron_ID")]
        public int DronID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public bool IsRent { get; set; }

        [Required]
        public CartItemStatus Status { get; set; }

        public DateTime AddedAt { get; set; }
        public DateTime? RemovedAt { get; set; }

        //  RELACIONES 
        [ForeignKey(nameof(CartID))]
        public Cart Cart { get; set; } = null!;

        [ForeignKey(nameof(DronID))]
        public Drone Drone { get; set; } = null!;

        // ===== PROPIEDADES SOLO UI =====

        [NotMapped]
        public bool Selected { get; set; } = false;

        [NotMapped]
        public decimal SubTotal => UnitPrice * Quantity;

        [NotMapped]
        public string PriceType => IsRent ? "Renta" : "Venta";
    }
}