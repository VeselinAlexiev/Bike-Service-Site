using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Data.Entities
{
    public class OrderBicycle
    {
        [Key, Column(Order = 0)]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Key, Column(Order = 1)]
        public int BicycleId { get; set; }
        public Bicycle Bicycle { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
