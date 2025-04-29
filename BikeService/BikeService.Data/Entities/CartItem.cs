using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BikeService.Data.Entities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        public int? PartId { get; set; }
        public int? BicycleId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public Cart Cart { get; set; }
        public SparePart SparePart { get; set; }
        public Bicycle Bicycle { get; set; }
    }
}