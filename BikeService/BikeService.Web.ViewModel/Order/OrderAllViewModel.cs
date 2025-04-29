using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeService.Data.Entities;

namespace BikeService.Web.ViewModel.Order
{
    public class OrderAllViewModel
    {
        public IEnumerable<BikeService.Data.Entities.Order> Orders { get; set; } = new List<BikeService.Data.Entities.Order>();

        public OrderFilterViewModel Filter { get; set; } = new OrderFilterViewModel();

        public bool HasMoreOrders { get; set; } = false;
    }
}
