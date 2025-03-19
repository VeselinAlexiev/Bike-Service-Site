using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Data.Entities
{
    public class SparePart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Compatibility { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [MaxLength(250)]
        public string ImageUrl { get; set; }

        public bool EcoFriendly { get; set; }

        public ICollection<OrderSparePart> OrderSpareParts { get; set; } = new List<OrderSparePart>();
        public ICollection<BicycleSparePart> BicycleSpareParts { get; set; } = new List<BicycleSparePart>();
    }
}
