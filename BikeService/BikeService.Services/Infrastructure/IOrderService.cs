using BikeService.Web.ViewModel.Order;
using BikeService.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BikeService.Services.Infrastructure
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(string userId, CheckoutViewModel checkoutData);
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task<List<Order>> GetAllOrdersFilteredAsync(OrderFilterViewModel filter);
        Task<int> CountOrdersAsync(OrderFilterViewModel filter);
        Task<Order> GetOrderDetailsAsync(int id);
        Task ChangeOrderStatusAsync(int orderId, string newStatus);
    }
}