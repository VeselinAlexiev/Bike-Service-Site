using BikeService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BikeService.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("AdminServiceType")]
    public class AdminServiceTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminServiceTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var services = await _context.ServiceTypes.ToListAsync();
            return View("~/Views/Admin/ServiceTypeIndex.cshtml", services);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/ServiceTypeCreate.cshtml", new ServiceType());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceType model)
        {
            if (!ModelState.IsValid) return View("~/Views/Admin/ServiceTypeCreate.cshtml", model);

            _context.ServiceTypes.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _context.ServiceTypes.FindAsync(id);
            if (service == null) return NotFound();

            return View("~/Views/Admin/ServiceTypeEdit.cshtml", service);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceType model)
        {
            if (!ModelState.IsValid) return View("~/Views/Admin/ServiceTypeEdit.cshtml", model);

            var existing = await _context.ServiceTypes.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.Description = model.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.ServiceTypes.FindAsync(id);
            if (service == null) return NotFound();

            _context.ServiceTypes.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
