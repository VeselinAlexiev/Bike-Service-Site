using BikeService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BikeService.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("AdminWorkshop")]
    public class AdminWorkshopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminWorkshopController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var workshops = await _context.Workshops.ToListAsync();
            return View("~/Views/Admin/WorkshopIndex.cshtml", workshops);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/WorkshopCreate.cshtml", new Workshop());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            var form = await Request.ReadFormAsync();

            var workshop = new Workshop
            {
                Name = form["Name"],
                Location = form["Location"],
                Latitude = double.TryParse(form["Latitude"], out var lat) ? lat : 0,
                Longitude = double.TryParse(form["Longitude"], out var lon) ? lon : 0
            };

            _context.Workshops.Add(workshop);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var workshop = await _context.Workshops.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
            if (workshop == null) return NotFound();

            return View("~/Views/Admin/WorkshopEdit.cshtml", workshop);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            var workshop = await _context.Workshops.FindAsync(id);
            if (workshop == null) return NotFound();

            var form = await Request.ReadFormAsync();

            workshop.Name = form["Name"];
            workshop.Location = form["Location"];
            workshop.Latitude = double.TryParse(form["Latitude"], out var lat) ? lat : 0;
            workshop.Longitude = double.TryParse(form["Longitude"], out var lon) ? lon : 0;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var workshop = await _context.Workshops.FindAsync(id);
            if (workshop == null) return NotFound();

            _context.Workshops.Remove(workshop);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}