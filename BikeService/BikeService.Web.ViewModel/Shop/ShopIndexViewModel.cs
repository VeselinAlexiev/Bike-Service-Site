using BikeService.Web.ViewModel.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Web.ViewModel.Shop
{
    public class ShopIndexViewModel
    {
        public IEnumerable<ShopItemViewModel> Items { get; set; } = new List<ShopItemViewModel>();

        public string? Category { get; set; }
        public string? SortOption { get; set; }
        public string? SearchTerm { get; set; }

        public int TotalResults { get; set; }

        public IEnumerable<string> Categories { get; set; } = new List<string>()
        {
            "Bicycle",
            "Spare Part"
        };

        public IEnumerable<string> SortOptions { get; set; } = new List<string>()
        {
            "Price Low-High",
            "Price High-Low",
            "Name A-Z",
            "Name Z-A",
            "Newest"
        };
    }
}
