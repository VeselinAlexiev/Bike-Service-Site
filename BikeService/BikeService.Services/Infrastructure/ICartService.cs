using BikeService.Web.ViewModel.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Services.Implementation
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, int itemId, int quantity);
        Task<IEnumerable<ShopItemViewModel>> GetCartItemsAsync(string userId);
        Task RemoveFromCartAsync(string userId, int itemId);
        Task ChangeQuantityAsync(string userId, int itemId, int delta);
    }
}
