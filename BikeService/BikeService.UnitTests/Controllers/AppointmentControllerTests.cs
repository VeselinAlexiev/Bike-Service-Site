using BikeService.Controllers;
using BikeService.Data;
using BikeService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using NETCore.MailKit.Core;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class AppointmentControllerTests
    {
        private ApplicationDbContext _context;
        private AppointmentController _controller;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IEmailService> _emailServiceMock;

        private ClaimsPrincipal CreateFakeUser(string userId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
        }

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null);

            _emailServiceMock = new Mock<IEmailService>();

            _controller = new AppointmentController(_context, _userManagerMock.Object, _emailServiceMock.Object);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Create_Should_Return_Unauthorized_If_User_Not_Logged()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.Create(1, 1, DateTime.Today, "10:00", "Test note");

            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Create_Should_Return_Error_If_Time_Invalid()
        {
            var userId = "user1";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateFakeUser(userId) }
            };

            var result = await _controller.Create(1, 1, DateTime.Today, "invalid-time", "Test note");

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Create_Should_Create_Appointment_When_Valid()
        {
            var userId = "user2";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateFakeUser(userId) }
            };

            var workshop = new Workshop { Id = 1, Name = "Workshop1", Location = "Sofia" };
            var bicycle = new Bicycle
            {
                Id = 1,
                Brand = "Brand",
                Model = "Model",
                Description = "Desc",
                EnergySource = "Manual",
                ImageUrl = "img.jpg",
                Material = "Aluminium",
                Type = "MTB"
            };

            await _context.Workshops.AddAsync(workshop);
            await _context.Bicycles.AddAsync(bicycle);
            await _context.SaveChangesAsync();

            var result = await _controller.Create(1, 1, DateTime.Today.AddDays(1), "10:00", "Test appointment");

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(_context.Appointments.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task MyAppointments_Should_Return_View_With_Appointments()
        {
            var userId = "user3";

            await _context.Appointments.AddAsync(new Appointment
            {
                Id = 1,
                UserId = userId,
                Workshop = new Workshop { Name = "WorkshopX", Location = "Y" },
                AppointmentDate = DateTime.Today.AddDays(1),
                Notes = "Check"
            });
            await _context.SaveChangesAsync();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateFakeUser(userId) }
            };

            var result = await _controller.MyAppointments();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Cancel_Should_Set_Status_To_Cancelled()
        {
            var userId = "user4";

            await _context.Appointments.AddAsync(new Appointment
            {
                Id = 2,
                UserId = userId,
                Workshop = new Workshop { Name = "WS", Location = "Loc" },
                AppointmentDate = DateTime.Today.AddDays(2),
                Notes = "Cancel test",
                Status = "Scheduled"
            });
            await _context.SaveChangesAsync();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateFakeUser(userId) }
            };

            var result = await _controller.Cancel(2);
            var appointment = await _context.Appointments.FindAsync(2);

            Assert.AreEqual("Cancelled", appointment.Status);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Edit_Should_Return_View_When_Valid()
        {
            var userId = "user5";

            await _context.Appointments.AddAsync(new Appointment
            {
                Id = 3,
                UserId = userId,
                Workshop = new Workshop { Name = "TestW", Location = "Loc" },
                AppointmentDate = DateTime.Today.AddDays(3),
                Notes = "Edit me"
            });
            await _context.SaveChangesAsync();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateFakeUser(userId) }
            };

            var result = await _controller.Edit(3);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task EditPost_Should_Update_When_Valid()
        {
            var userId = "user6";

            await _context.Appointments.AddAsync(new Appointment
            {
                Id = 4,
                UserId = userId,
                AppointmentDate = DateTime.Today.AddDays(5),
                Notes = "Edit Post"
            });
            await _context.SaveChangesAsync();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateFakeUser(userId) }
            };

            var newDate = DateTime.Today.AddDays(7);

            var result = await _controller.Edit(4, newDate, "11:00");

            var updated = await _context.Appointments.FindAsync(4);

            Assert.AreEqual(newDate.Date.AddHours(11), updated.AppointmentDate);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
    }
}