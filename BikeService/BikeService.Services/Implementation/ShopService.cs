using BikeService.Web.ViewModel.Details;
using BikeService.Web.ViewModel.Shop;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Services.Implementation
{
    public class ShopService : IShopService
    {
        private readonly ApplicationDbContext _context;

        public ShopService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShopItemViewModel>> GetAllItemsAsync(string? category = null, string? sortOption = null)
        {
            var bicycleEntities = await _context.Bicycles.ToListAsync();
            var sparePartEntities = await _context.SpareParts.ToListAsync();

            var bicycleItems = bicycleEntities.Select(b => new ShopItemViewModel
            {
                Id = b.Id,
                Name = $"{b.Brand} {b.Model}",
                Description = b.Description,
                Price = b.Price,
                ImageUrl = b.ImageUrl ?? string.Empty,
                Category = b.Type,
                Type = "Bicycle"
            });

            var sparePartItems = sparePartEntities.Select(sp => new ShopItemViewModel
            {
                Id = sp.Id,
                Name = sp.Name,
                Description = sp.Description,
                Price = sp.Price,
                ImageUrl = sp.ImageUrl ?? string.Empty,
                Category = sp.Compatibility ?? "Other",
                Type = "SparePart"
            });

            var items = bicycleItems.Concat(sparePartItems);

            if (!string.IsNullOrEmpty(category))
            {
                items = items.Where(i => i.Category == category);
            }

            items = sortOption switch
            {
                "PriceLowHigh" => items.OrderBy(i => i.Price),
                "PriceHighLow" => items.OrderByDescending(i => i.Price),
                "NameAZ" => items.OrderBy(i => i.Name),
                "NameZA" => items.OrderByDescending(i => i.Name),
                "Newest" => items.OrderByDescending(i => i.Id),
                _ => items
            };

            return items.ToList();
        }

        public async Task<ProductDetailsViewModel?> GetItemByIdAsync(int id)
        {
            var bicycle = await _context.Bicycles.FindAsync(id);
            if (bicycle != null)
            {
                return new ProductDetailsViewModel
                {
                    Id = bicycle.Id,
                    Name = $"{bicycle.Brand} {bicycle.Model}",
                    Description = bicycle.Description,
                    ImageUrl = bicycle.ImageUrl ?? string.Empty,
                    Price = bicycle.Price,
                    Category = bicycle.Type,
                    Type = "Bicycle",
                    Brand = bicycle.Brand,
                    Year = bicycle.Year,
                    Material = bicycle.Material,
                    BatteryCapacity = bicycle.BatteryCapacity,
                    EnergySource = bicycle.EnergySource,
                    EcoFriendly = bicycle.EcoFriendly,
                    StockQuantity = 0
                };
            }

            var sparePart = await _context.SpareParts.FindAsync(id);
            if (sparePart != null)
            {
                return new ProductDetailsViewModel
                {
                    Id = sparePart.Id,
                    Name = sparePart.Name,
                    Description = sparePart.Description,
                    ImageUrl = sparePart.ImageUrl ?? string.Empty,
                    Price = sparePart.Price,
                    Category = sparePart.Compatibility ?? "Other",
                    Type = "SparePart",
                    StockQuantity = sparePart.StockQuantity,
                    EcoFriendly = sparePart.EcoFriendly,
                    Brand = string.Empty,
                    Year = 0,
                    Material = string.Empty,
                    BatteryCapacity = null,
                    EnergySource = string.Empty
                };
            }

            return null;
        }
    }
}