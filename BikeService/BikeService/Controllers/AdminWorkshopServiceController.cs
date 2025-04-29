using BikeService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BikeService.Controllers
{
        [Authorize(Roles = "Admin")]
        [Route("AdminWorkshopService")]
        public class AdminWorkshopServiceController : Controller
        {
            private readonly ApplicationDbContext _context;

            public AdminWorkshopServiceController(ApplicationDbContext context)
            {
                _context = context;
            }

            [HttpGet("Index")]
            public async Task<IActionResult> Index()
            {
                var links = await _context.WorkshopServices
                    .Include(ws => ws.Workshop)
                    .Include(ws => ws.ServiceType)
                    .ToListAsync();

                return View("~/Views/Admin/WorkshopServiceIndex.cshtml", links);
            }

            [HttpGet("Create")]
            public async Task<IActionResult> Create()
            {
                ViewBag.Workshops = await _context.Workshops.ToListAsync();
                ViewBag.ServiceTypes = await _context.ServiceTypes.ToListAsync();
                return View("~/Views/Admin/WorkshopServiceCreate.cshtml", new WorkshopService());
            }

            [HttpPost("Create")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(WorkshopService model)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Workshops = await _context.Workshops.ToListAsync();
                    ViewBag.ServiceTypes = await _context.ServiceTypes.ToListAsync();
                    return View("~/Views/Admin/WorkshopServiceCreate.cshtml", model);
                }

                _context.WorkshopServices.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            [HttpPost("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(int workshopId, int serviceTypeId)
            {
                var link = await _context.WorkshopServices.FindAsync(workshopId, serviceTypeId);
                if (link == null) return NotFound();

                _context.WorkshopServices.Remove(link);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
}
