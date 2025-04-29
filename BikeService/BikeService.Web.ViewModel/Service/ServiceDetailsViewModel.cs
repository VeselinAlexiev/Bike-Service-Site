using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Web.ViewModel.Service
{
    public class ServiceDetailsViewModel
    {
        public int WorkshopId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<ServiceItem> Services { get; set; } = new();
        public List<BicycleModelInfo> BicycleModels { get; set; } = new();

        public class ServiceItem
        {
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public TimeSpan TimeRequired { get; set; }
        }

        public class BicycleModelInfo
        {
            public int Id { get; set; }
            public string Model { get; set; } = string.Empty;
            public string Brand { get; set; } = string.Empty;
            public int Year { get; set; }
            public string Type { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
        }
    }
}
