using BikeService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BikeService.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public AppointmentController(ApplicationDbContext context, UserManager<User> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int WorkshopId, int BicycleId, DateTime AppointmentDate, string Time, string? Notes)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            if (!TimeSpan.TryParse(Time, out var parsedTime))
            {
                TempData["Error"] = "Invalid time format selected.";
                return RedirectToAction("Details", "Service", new { id = WorkshopId });
            }

            var fullDateTime = AppointmentDate.Date.Add(parsedTime);

            if (parsedTime.Hours < 9 || parsedTime.Hours > 17)
            {
                TempData["Error"] = "Selected time is outside working hours (9:00–17:00).";
                return RedirectToAction("Details", "Service", new { id = WorkshopId });
            }

            bool appointmentExists = await _context.Appointments
                .AnyAsync(a => a.UserId == userId && a.AppointmentDate == fullDateTime);

            if (appointmentExists)
            {
                TempData["Error"] = "You already have an appointment at the selected time.";
                return RedirectToAction("Details", "Service", new { id = WorkshopId });
            }

            var appointment = new Appointment
            {
                UserId = userId,
                WorkshopId = WorkshopId,
                AppointmentDate = fullDateTime,
                Status = "Scheduled",
                Notes = Notes
            };

            var appointmentBicycle = new AppointmentBicycle
            {
                BicycleId = BicycleId,
                Appointment = appointment
            };

            _context.Appointments.Add(appointment);
            _context.AppointmentBicycles.Add(appointmentBicycle);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Your appointment was successfully scheduled.";
            return RedirectToAction("Details", "Service", new { id = WorkshopId });
        }

        [Authorize]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Workshop)
                .Include(a => a.AppointmentBicycles)
                    .ThenInclude(ab => ab.Bicycle)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
                return Unauthorized();

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment cancelled successfully.";
            return RedirectToAction("MyAppointments");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointment = await _context.Appointments
                .Include(a => a.Workshop)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
                return Unauthorized();

            if ((appointment.AppointmentDate.Date - DateTime.Today).TotalDays < 2)
            {
                TempData["Error"] = "Appointments can only be edited at least 2 days in advance.";
                return RedirectToAction("MyAppointments");
            }

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DateTime newDate, string newTime)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
                return Unauthorized();

            if ((appointment.AppointmentDate.Date - DateTime.Today).TotalDays < 2)
            {
                TempData["Error"] = "Appointments can only be edited at least 2 days in advance.";
                return RedirectToAction("MyAppointments");
            }

            if (!TimeSpan.TryParse(newTime, out var parsedTime))
            {
                TempData["Error"] = "Invalid time selected.";
                return RedirectToAction("Edit", new { id });
            }

            if (parsedTime.Hours < 9 || parsedTime.Hours > 17)
            {
                TempData["Error"] = "Time must be between 09:00 and 17:00.";
                return RedirectToAction("Edit", new { id });
            }

            if (newDate.DayOfWeek == DayOfWeek.Saturday || newDate.DayOfWeek == DayOfWeek.Sunday)
            {
                TempData["Error"] = "Appointments can only be scheduled on weekdays.";
                return RedirectToAction("Edit", new { id });
            }

            if (newDate < DateTime.Today)
            {
                TempData["Error"] = "Date must be in the future.";
                return RedirectToAction("Edit", new { id });
            }

            appointment.AppointmentDate = newDate.Date.Add(parsedTime);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment updated successfully.";
            return RedirectToAction("MyAppointments");
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> AdminView(int? selectedWorkshopId = null, DateTime? date = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.Include(u => u.Workshop).FirstOrDefaultAsync(u => u.Id == userId);

            IQueryable<Appointment> appointmentsQuery = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Workshop)
                .Include(a => a.AppointmentBicycles)
                    .ThenInclude(ab => ab.Bicycle);

            if (User.IsInRole("Employee"))
            {
                if (user?.WorkshopId == null)
                {
                    TempData["Error"] = "You are not assigned to any workshop.";
                    return RedirectToAction("Index", "Home");
                }

                appointmentsQuery = appointmentsQuery.Where(a => a.WorkshopId == user.WorkshopId);
            }

            if (User.IsInRole("Admin") && selectedWorkshopId.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.WorkshopId == selectedWorkshopId.Value);
            }

            if (date.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            var workshops = await _context.Workshops
                .OrderBy(w => w.Name)
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.Name
                })
                .ToListAsync();

            ViewBag.Workshops = workshops;
            ViewBag.SelectedWorkshopId = selectedWorkshopId;

            var appointments = await appointmentsQuery
                .OrderBy(a => a.AppointmentDate)
                .Take(10)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Workshop)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToAction("AdminView");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.Include(u => u.Workshop).FirstOrDefaultAsync(u => u.Id == userId);

            if (User.IsInRole("Employee") && user.WorkshopId != appointment.WorkshopId)
            {
                TempData["Error"] = "You don't have permission to modify this appointment.";
                return RedirectToAction("AdminView");
            }

            appointment.Status = newStatus;
            await _context.SaveChangesAsync();

            await _emailService.SendAsync(appointment.User.Email,
                "Appointment Status Updated",
                $"<p>Your appointment at <strong>{appointment.Workshop?.Name}</strong> has been updated to <strong>{newStatus}</strong>.</p>",
                true);

            TempData["Success"] = "Appointment status updated and user notified.";
            return RedirectToAction("AdminView");
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Workshop)
                .Include(a => a.AppointmentBicycles)
                    .ThenInclude(ab => ab.Bicycle)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToAction("AdminView");
            }

            return View(appointment);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        public async Task<IActionResult> LoadMoreAppointments(int currentPage, int? selectedWorkshopId = null, DateTime? date = null)
        {
            int pageSize = 10;
            var query = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Workshop)
                .Include(a => a.AppointmentBicycles)
                    .ThenInclude(ab => ab.Bicycle)
                .OrderBy(a => a.AppointmentDate)
                .AsQueryable();

            if (selectedWorkshopId.HasValue)
            {
                query = query.Where(a => a.WorkshopId == selectedWorkshopId.Value);
            }

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            var appointments = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!appointments.Any())
            {
                return Content("");
            }

            return PartialView("_AppointmentRowPartial", appointments);
        }
    }
}