using BikeService.Services.Implementation;
using BikeService.Web.ViewModel.Details;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace BikeService.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IShopService _shopService;
        private readonly ICartService _cartService;

        public ProductController(IShopService shopService, ICartService cartService)
        {
            _shopService = shopService;
            _cartService = cartService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _shopService.GetItemByIdAsync(id);
            if (item == null) return NotFound();

            var model = item;
            model.Quantity = 1;

            var allItems = await _shopService.GetAllItemsAsync();
            var relatedItems = allItems
                .Where(p => p.Category == item.Category && p.Id != item.Id)
                .Take(4)
                .Select(p => new ProductDetailsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Category = p.Category,
                    Type = p.Type
                }).ToList();

            ViewBag.RelatedItems = relatedItems;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(ProductDetailsViewModel model)
        {
            if (!ModelState.IsValid) return View("Details", model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _cartService.AddToCartAsync(userId, model.Id, model.Quantity);

            return RedirectToAction("Cart", "Shop");
        }
    }
}