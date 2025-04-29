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
                Bicycles = _context.Bicycles
                    .OrderByDescending(b => b.Price)
                    .Take(3)
                    .Select(b => new ProductViewModel
                    {
                        Id = b.Id,
                        Name = b.Model,
                        ImageUrl = b.ImageUrl,
                        Price = b.Price
                    })
                    .ToList(),

                SpareParts = _context.SpareParts
                    .OrderByDescending(sp => sp.Price)
                    .Take(3)
                    .Select(sp => new ProductViewModel
                    {
                        Id = sp.Id,
                        Name = sp.Name,
                        ImageUrl = sp.ImageUrl,
                        Price = sp.Price
                    })
                    .ToList(),
            };

            return View(viewModel);
        }
    }
}