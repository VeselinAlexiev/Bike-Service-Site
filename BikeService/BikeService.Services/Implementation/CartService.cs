using BikeService.Data.Entities;
using BikeService.Web.ViewModel.Shop;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeService.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(string userId, int itemId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var sparePart = await _context.SpareParts.FindAsync(itemId);
            var bicycle = await _context.Bicycles.FindAsync(itemId);

            if (sparePart == null && bicycle == null)
                return;

            CartItem existingItem = null;

            if (sparePart != null)
            {
                existingItem = cart.Items.FirstOrDefault(i => i.PartId == itemId);
            }
            else if (bicycle != null)
            {
                existingItem = cart.Items.FirstOrDefault(i => i.BicycleId == itemId);
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    PartId = sparePart?.Id,
                    BicycleId = bicycle?.Id,
                    Price = sparePart?.Price ?? bicycle.Price,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShopItemViewModel>> GetCartItemsAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.SparePart)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Bicycle)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return new List<ShopItemViewModel>();

            return cart.Items.Select(i => new ShopItemViewModel
            {
                Id = i.PartId ?? i.BicycleId ?? 0,
                Name = i.PartId.HasValue ? i.SparePart?.Name : i.Bicycle?.Model,
                Description = i.PartId.HasValue ? i.SparePart?.Description : i.Bicycle?.Description,
                Price = i.Price,
                ImageUrl = i.PartId.HasValue ? i.SparePart?.ImageUrl ?? "" : i.Bicycle?.ImageUrl ?? "",
                Category = i.PartId.HasValue ? i.SparePart?.Compatibility ?? "Unknown" : i.Bicycle?.Type ?? "Unknown",
                Type = i.PartId.HasValue ? "SparePart" : "Bicycle",
                Quantity = i.Quantity
            });
        }

        public async Task RemoveFromCartAsync(string userId, int itemId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return;

            var item = cart.Items
                .FirstOrDefault(i => (i.PartId == itemId) || (i.BicycleId == itemId));

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ChangeQuantityAsync(string userId, int itemId, int delta)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return;

            var item = cart.Items
                .FirstOrDefault(i => (i.PartId == itemId) || (i.BicycleId == itemId));

            if (item == null) return;

            item.Quantity += delta;

            if (item.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }

            await _context.SaveChangesAsync();
        }
    }
}
