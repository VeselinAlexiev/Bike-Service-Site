using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Data.Entities
{
    public class WorkshopService
    {
        public int WorkshopId { get; set; }
        public Workshop Workshop { get; set; }

        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }

        public decimal Price { get; set; }
        public TimeSpan TimeRequired { get; set; }
    }
}
