using BikeService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BikeService.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("AdminBicycle")]
    public class AdminBicycleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminBicycleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var bikes = await _context.Bicycles.ToListAsync();
            return View("~/Views/Admin/BicycleIndex.cshtml", bikes);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/BicycleCreate.cshtml", new Bicycle());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            var form = await Request.ReadFormAsync();

            var bicycle = new Bicycle
            {
                Model = form["Model"],
                Brand = form["Brand"],
                Year = int.TryParse(form["Year"], out int y) ? y : 0,
                Type = form["Type"],
                Price = decimal.TryParse(form["Price"], out decimal p) ? p : 0,
                Description = form["Description"],
                ImageUrl = form["ImageUrl"],
                Material = form["Material"],
                BatteryCapacity = decimal.TryParse(form["BatteryCapacity"], out decimal b) ? b : null,
                EnergySource = form["EnergySource"],
                EcoFriendly = form["EcoFriendly"] == "true" || form["EcoFriendly"] == "on"
            };

            _context.Bicycles.Add(bicycle);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var bike = await _context.Bicycles.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            if (bike == null) return NotFound();

            return View("~/Views/Admin/BicycleEdit.cshtml", bike);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            var bicycle = await _context.Bicycles.FindAsync(id);
            if (bicycle == null) return NotFound();

            var form = await Request.ReadFormAsync();

            bicycle.Model = form["Model"];
            bicycle.Brand = form["Brand"];
            bicycle.Year = int.TryParse(form["Year"], out int y) ? y : 0;
            bicycle.Type = form["Type"];
            bicycle.Price = decimal.TryParse(form["Price"], out decimal p) ? p : 0;
            bicycle.Description = form["Description"];
            bicycle.ImageUrl = form["ImageUrl"];
            bicycle.Material = form["Material"];
            bicycle.BatteryCapacity = decimal.TryParse(form["BatteryCapacity"], out decimal b) ? b : null;
            bicycle.EnergySource = form["EnergySource"];
            bicycle.EcoFriendly = form["EcoFriendly"] == "true" || form["EcoFriendly"] == "on";

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var bike = await _context.Bicycles.FindAsync(id);
            if (bike == null) return NotFound();

            _context.Bicycles.Remove(bike);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}