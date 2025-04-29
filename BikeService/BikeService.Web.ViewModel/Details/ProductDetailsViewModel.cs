using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Web.ViewModel.Details
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public bool EcoFriendly { get; set; }
        public bool IsUniversal => (Category?.ToLower().Contains("universal") ?? false);

        public string Brand { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Material { get; set; } = string.Empty;
        public decimal? BatteryCapacity { get; set; }
        public string EnergySource { get; set; } = string.Empty;

        public int StockQuantity { get; set; }
    }
}
