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

        public bool IsSale => !IsRent;

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

        // ===== PROPIEDADES PARA RENTA =====

        public DateTime? RentStartDate { get; set; }

        public DateTime? RentEndDate { get; set; }

        // Precio diario congelado al momento de agregar al carrito
        public decimal? RentPricePerDay { get; set; }

        [NotMapped]
        public int RentDays =>
           (IsRent && RentStartDate.HasValue && RentEndDate.HasValue)
            ? (RentEndDate.Value.Date - RentStartDate.Value.Date).Days + 1
            : 0;

        [NotMapped]
        public decimal RentTotal =>
            (IsRent && RentPricePerDay.HasValue)
                ? RentDays * RentPricePerDay.Value * Quantity
                : 0;

        public DroneAvailability AvailabilityMode { get; set; }
    }


}