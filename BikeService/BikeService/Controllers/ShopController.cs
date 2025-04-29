using BikeService.Services.Implementation;
using BikeService.Web.ViewModel.Details;
using BikeService.Web.ViewModel.Shop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BikeService.Controllers
{
    public class ShopController : Controller
    {
        private readonly IShopService _shopService;
        private readonly ICartService _cartService;

        public ShopController(IShopService shopService, ICartService cartService)
        {
            _shopService = shopService;
            _cartService = cartService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? category, string? sortOption, string? searchTerm)
        {
            var items = await _shopService.GetAllItemsAsync(category, sortOption);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(searchTerm) ||
                                         i.Description.ToLower().Contains(searchTerm));
            }

            var viewModel = new ShopIndexViewModel
            {
                Items = items,
                Category = category,
                SortOption = sortOption,
                SearchTerm = searchTerm,
                TotalResults = items.Count()
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _shopService.GetItemByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(ProductDetailsViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = Url.Action("Details", "Shop", new { id = model.Id }) });
            }

            await _cartService.AddToCartAsync(userId, model.Id, model.Quantity);

            return RedirectToAction("Cart");
        }

        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var items = await _cartService.GetCartItemsAsync(userId);
            return View(items);
        }

        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _cartService.RemoveFromCartAsync(userId, id);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _cartService.ChangeQuantityAsync(userId, id, 1);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _cartService.ChangeQuantityAsync(userId, id, -1);
            return RedirectToAction("Cart");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ClearFilters()
        {
            return RedirectToAction("Index");
        }
    }
}