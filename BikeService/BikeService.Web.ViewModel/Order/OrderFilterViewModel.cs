using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Web.ViewModel.Order
{
    public class OrderFilterViewModel
    {
        public string? SearchEmail { get; set; }
        public string? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
