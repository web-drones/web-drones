using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Drones_Proyect.Models
{
    public class Category
    {
        [Key]
        [Column("Categoria_ID")]
        public int CategoryID { get; set; }
        [Required]
        [Column("Nombre")]
        public string Name { get; set; } = string.Empty;
    }
}
