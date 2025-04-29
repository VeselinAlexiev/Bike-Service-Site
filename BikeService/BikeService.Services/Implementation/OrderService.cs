using BikeService.Data.Entities;
using BikeService.Services.Infrastructure;
using BikeService.Web.ViewModel.Order;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeService.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public OrderService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<int> CreateOrderAsync(string userId, CheckoutViewModel checkoutData)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.SparePart)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Bicycle)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            decimal totalAmount = cart.Items.Sum(i => i.Price * i.Quantity);

            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                Status = "Pending",
                Address = checkoutData.Address,
                Phone = checkoutData.Phone,
                PaymentMethod = checkoutData.PaymentMethod
            };

            foreach (var item in cart.Items)
            {
                if (item.PartId.HasValue)
                {
                    order.OrderSpareParts.Add(new OrderSparePart
                    {
                        PartId = item.PartId.Value,
                        Quantity = item.Quantity
                    });
                }
                else if (item.BicycleId.HasValue)
                {
                    order.OrderBicycles.Add(new OrderBicycle
                    {
                        BicycleId = item.BicycleId.Value,
                        Quantity = item.Quantity
                    });
                }
            }

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                await _emailService.SendAsync(
                    user.Email,
                    "Your BikeService Order Confirmation",
                    $"<p>Dear {user.UserName},</p>" +
                    $"<p>Your order (ID: {order.Id}) has been successfully placed!</p>" +
                    $"<p><strong>Total:</strong> {order.TotalAmount:C}</p>" +
                    $"<p><strong>Status:</strong> {order.Status}</p>" +
                    $"<p><strong>Address:</strong> {order.Address}</p>" +
                    $"<p><strong>Phone:</strong> {order.Phone}</p>" +
                    "<br/><p>Thank you for choosing BikeService!</p>",
                    true
                );
            }

            return order.Id;
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderSpareParts)
                    .ThenInclude(osp => osp.SparePart)
                .Include(o => o.OrderBicycles)
                    .ThenInclude(ob => ob.Bicycle)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersFilteredAsync(OrderFilterViewModel filter)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderSpareParts)
                    .ThenInclude(osp => osp.SparePart)
                .Include(o => o.OrderBicycles)
                    .ThenInclude(ob => ob.Bicycle)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.SearchEmail))
            {
                query = query.Where(o => o.User.Email.Contains(filter.SearchEmail));
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(o => o.Status == filter.Status);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(o => o.OrderDate.Date >= filter.DateFrom.Value.Date);
            }

            if (filter.DateTo.HasValue)
            {
                query = query.Where(o => o.OrderDate.Date <= filter.DateTo.Value.Date);
            }

            return await query
                .OrderBy(o => o.Status == "Processing" ? 0 :
                              o.Status == "Pending" ? 1 :
                              o.Status == "Completed" ? 2 : 3)
                .ThenByDescending(o => o.OrderDate)
                .Skip((filter.CurrentPage - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<int> CountOrdersAsync(OrderFilterViewModel filter)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(filter.SearchEmail))
            {
                query = query.Include(o => o.User)
                             .Where(o => o.User.Email.Contains(filter.SearchEmail));
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(o => o.Status == filter.Status);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(o => o.OrderDate.Date >= filter.DateFrom.Value.Date);
            }

            if (filter.DateTo.HasValue)
            {
                query = query.Where(o => o.OrderDate.Date <= filter.DateTo.Value.Date);
            }

            return await query.CountAsync();
        }

        public async Task<Order> GetOrderDetailsAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderSpareParts)
                    .ThenInclude(osp => osp.SparePart)
                .Include(o => o.OrderBicycles)
                    .ThenInclude(ob => ob.Bicycle)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            return order;
        }

        public async Task ChangeOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            if (order.User != null)
            {
                await _emailService.SendAsync(
                    order.User.Email,
                    "Your BikeService Order Status Updated",
                    $"<p>Dear {order.User.UserName},</p>" +
                    $"<p>Your order (ID: {order.Id}) status has been updated.</p>" +
                    $"<p><strong>New Status:</strong> {order.Status}</p>" +
                    $"<br/><p>Thank you for using BikeService!</p>",
                    true
                );
            }
        }
    }
}