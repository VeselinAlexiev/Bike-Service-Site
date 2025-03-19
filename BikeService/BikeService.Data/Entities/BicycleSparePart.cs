using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Data.Entities
{
    public class BicycleSparePart
    {
        [Key]
        public int Id { get; set; }

        public int BicycleId { get; set; }
        public Bicycle Bicycle { get; set; }

        public int SparePartId { get; set; }
        public SparePart SparePart { get; set; }
    }
}
