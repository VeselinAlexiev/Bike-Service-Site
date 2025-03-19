using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Data.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
