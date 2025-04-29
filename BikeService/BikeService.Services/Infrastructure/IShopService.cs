using BikeService.Web.ViewModel.Details;
using BikeService.Web.ViewModel.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Services.Implementation
{
    public interface IShopService
    {
        Task<IEnumerable<ShopItemViewModel>> GetAllItemsAsync(string? category = null, string? sortOption = null);
        Task<ProductDetailsViewModel?> GetItemByIdAsync(int id);
    }
}
