using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using BikeService.Web.ViewModel.Home;
using BikeService.Web.ViewModel;

namespace BikeService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                // Get top 3 bicycles ordered by price descending
                Bicycles = _context.Bicycles
                    .OrderByDescending(b => b.Price)  // Order by price descending
                    .Take(3)  // Get top 3
                    .Select(b => new ProductViewModel
                    {
                        Id = b.Id,
                        Name = b.Model,
                        ImageUrl = b.ImageUrl,
                        Price = b.Price  // Include Price
                    })
                    .ToList(),

                // Get top 3 spare parts ordered by price descending
                SpareParts = _context.SpareParts
                    .OrderByDescending(sp => sp.Price)  // Order by price descending
                    .Take(3)  // Get top 3
                    .Select(sp => new ProductViewModel
                    {
                        Id = sp.Id,
                        Name = sp.Name,
                        ImageUrl = sp.ImageUrl,
                        Price = sp.Price  // Include Price
                    })
                    .ToList(),
            };

            return View(viewModel);
        }
    }
}