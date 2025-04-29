using BikeService.Data;
using BikeService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BikeService.Web.ViewModel;
using BikeService.Web.ViewModel.User;

namespace BikeService.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminUserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new AdminUserViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            ViewBag.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            ViewBag.AllWorkshops = await _context.Workshops.Select(w => w.Name).ToListAsync();

            return View("~/Views/Admin/UserIndex.cshtml", userViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string userId, string role, string? workshopName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (role == "Admin")
                return BadRequest("Cannot assign Admin role.");

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            if (role == "Employee" && !string.IsNullOrEmpty(workshopName))
            {
                var workshop = await _context.Workshops.FirstOrDefaultAsync(w => w.Name == workshopName);
                if (workshop != null)
                {
                    user.WorkshopId = workshop.Id;
                    await _userManager.UpdateAsync(user);
                }
            }

            TempData["Success"] = "Role added successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (role == "Admin")
                return BadRequest("Cannot remove Admin role.");

            await _userManager.RemoveFromRoleAsync(user, role);

            if (role == "Employee")
            {
                user.WorkshopId = null;
                await _userManager.UpdateAsync(user);
            }

            TempData["Success"] = "Role removed successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                return BadRequest("Cannot delete an Admin user.");

            await _userManager.DeleteAsync(user);

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}