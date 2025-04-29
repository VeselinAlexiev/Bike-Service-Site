using BikeService.Services.Implementation;
using BikeService.Services.Infrastructure;
using BikeService.Web.ViewModel.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BikeService.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = (await _cartService.GetCartItemsAsync(userId)).ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Shop");
            }

            var viewModel = new CheckoutViewModel
            {
                TotalAmount = cartItems.Sum(i => i.Price * i.Quantity),
                TotalItems = cartItems.Sum(i => i.Quantity)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCheckout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return View("Checkout", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var orderId = await _orderService.CreateOrderAsync(userId, model);
                TempData["Success"] = $"Your order #{orderId} was successfully placed!";
                return RedirectToAction(nameof(MyOrders));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error placing order: {ex.Message}";
                return RedirectToAction("Cart", "Shop");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _orderService.GetUserOrdersAsync(userId);

            return View(orders);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        public async Task<IActionResult> AllOrders([FromQuery] OrderFilterViewModel filter)
        {
            if (filter == null)
            {
                filter = new OrderFilterViewModel();
            }

            var orders = await _orderService.GetAllOrdersFilteredAsync(filter);

            var totalOrders = await _orderService.CountOrdersAsync(filter);
            var hasMore = totalOrders > filter.CurrentPage * filter.PageSize;

            var viewModel = new OrderAllViewModel
            {
                Orders = orders,
                Filter = filter,
                HasMoreOrders = hasMore
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            try
            {
                await _orderService.ChangeOrderStatusAsync(id, newStatus);
                TempData["Success"] = "Order status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update status: {ex.Message}";
            }

            return RedirectToAction(nameof(AllOrders));
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction(nameof(AllOrders));
            }

            return View(order);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        public async Task<IActionResult> LoadMoreOrders(int currentPage)
        {
            var filter = new OrderFilterViewModel
            {
                CurrentPage = currentPage
            };

            var orders = await _orderService.GetAllOrdersFilteredAsync(filter);

            if (!orders.Any())
            {
                return Content("");
            }

            return PartialView("_OrderRowPartial", orders);
        }

    }
}