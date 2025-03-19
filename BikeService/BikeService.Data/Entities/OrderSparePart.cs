using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Data.Entities
{
    public class OrderSparePart
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int PartId { get; set; }
        public SparePart SparePart { get; set; }
        public int Quantity { get; set; }
    }
}
