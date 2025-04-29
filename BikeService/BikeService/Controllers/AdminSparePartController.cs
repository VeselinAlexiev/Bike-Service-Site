using BikeService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BikeService.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("AdminSparePart")]
    public class AdminSparePartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSparePartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var parts = await _context.SpareParts.ToListAsync();
            return View("~/Views/Admin/SparePartIndex.cshtml", parts);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/SparePartCreate.cshtml", new SparePart());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            var form = await Request.ReadFormAsync();

            var part = new SparePart
            {
                Name = form["Name"],
                Description = form["Description"],
                Price = decimal.TryParse(form["Price"], out var price) ? price : 0,
                Compatibility = form["Compatibility"],
                StockQuantity = int.TryParse(form["StockQuantity"], out var qty) ? qty : 0,
                ImageUrl = form["ImageUrl"],
                EcoFriendly = form["EcoFriendly"] == "true" || form["EcoFriendly"] == "on"
            };

            _context.SpareParts.Add(part);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var part = await _context.SpareParts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (part == null) return NotFound();

            return View("~/Views/Admin/SparePartEdit.cshtml", part);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            var part = await _context.SpareParts.FindAsync(id);
            if (part == null) return NotFound();

            var form = await Request.ReadFormAsync();

            part.Name = form["Name"];
            part.Description = form["Description"];
            part.Price = decimal.TryParse(form["Price"], out var price) ? price : 0;
            part.Compatibility = form["Compatibility"];
            part.StockQuantity = int.TryParse(form["StockQuantity"], out var qty) ? qty : 0;
            part.ImageUrl = form["ImageUrl"];
            part.EcoFriendly = form["EcoFriendly"] == "true" || form["EcoFriendly"] == "on";

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var part = await _context.SpareParts.FindAsync(id);
            if (part == null) return NotFound();

            _context.SpareParts.Remove(part);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
