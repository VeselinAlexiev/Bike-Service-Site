using BikeService.Data.Entities;
using BikeService.Web.ViewModel.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var services = await _context.Workshops
                .Include(w => w.WorkshopServices)
                    .ThenInclude(ws => ws.ServiceType)
                .Select(w => new ServiceViewModel
                {
                    Id = w.Id,
                    WorkshopName = w.Name,
                    Location = w.Location,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude,

                    ServiceType = string.Join(", ", w.WorkshopServices
                        .Select(ws => ws.ServiceType.Title).Distinct()),

                    Description = string.Join(" | ", w.WorkshopServices
                        .Select(ws => ws.ServiceType.Description).Distinct()),

                    Price = w.WorkshopServices.FirstOrDefault() != null
                        ? w.WorkshopServices.First().Price
                        : 0,

                    TimeRequired = w.WorkshopServices.FirstOrDefault() != null
                        ? w.WorkshopServices.First().TimeRequired
                        : TimeSpan.Zero
                })
                .ToListAsync();

            return View("Index", services);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var workshop = await _context.Workshops
                .Include(w => w.WorkshopServices)
                    .ThenInclude(ws => ws.ServiceType)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workshop == null)
            {
                return NotFound();
            }

            var bicycles = await _context.Bicycles.ToListAsync();

            var model = new ServiceDetailsViewModel
            {
                WorkshopId = workshop.Id,
                Name = workshop.Name,
                Location = workshop.Location,
                Latitude = workshop.Latitude,
                Longitude = workshop.Longitude,
                Services = workshop.WorkshopServices.Select(ws => new ServiceDetailsViewModel.ServiceItem
                {
                    Title = ws.ServiceType.Title,
                    Description = ws.ServiceType.Description,
                    TimeRequired = ws.TimeRequired,
                    Price = ws.Price
                }).ToList(),
                BicycleModels = bicycles.Select(b => new ServiceDetailsViewModel.BicycleModelInfo
                {
                    Id = b.Id,
                    Model = b.Model,
                    Brand = b.Brand,
                    Year = b.Year,
                    Type = b.Type,
                    ImageUrl = b.ImageUrl
                }).ToList()
            };

            return View("Details", model);
        }
    }
}