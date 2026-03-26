using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_Drones_Proyect.Enums;

namespace Web_Drones_Proyect.Models
{
    public class Cart
    {
        [Key]
        [Column("Cart_ID")]
        public int CartID { get; set; }

        [Required]
        [Column("Usuario_ID")]
        public int UserID { get; set; }

        [Required]
        public CartStatus Status { get; set; } = CartStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; } = null!;
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}