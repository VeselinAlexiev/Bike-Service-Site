using BikeService.Controllers;
using BikeService.Data;
using BikeService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class AdminWorkshopServiceControllerTests
    {
        private ApplicationDbContext _context;
        private AdminWorkshopServiceController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _controller = new AdminWorkshopServiceController(_context);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Role, "Admin")
                    }, "mock"))
                }
            };
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_Should_Return_View_With_WorkshopServices()
        {
            // Arrange
            await _context.WorkshopServices.AddAsync(new WorkshopService
            {
                WorkshopId = 1,
                ServiceTypeId = 1,
                Price = 100,
                TimeRequired = TimeSpan.FromHours(1)
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult.Model);
        }

        [Test]
        public async Task Create_Get_Should_Return_View()
        {
            // Act
            var result = await _controller.Create();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Create_Post_Should_Create_WorkshopService_When_ModelValid()
        {
            // Arrange
            var workshop = new Workshop { Id = 1, Name = "Test Workshop", Location = "Sofia" };
            var serviceType = new ServiceType { Id = 1, Title = "Repair", Description = "Repair bikes" };
            await _context.Workshops.AddAsync(workshop);
            await _context.ServiceTypes.AddAsync(serviceType);
            await _context.SaveChangesAsync();

            var model = new WorkshopService
            {
                WorkshopId = 1,
                ServiceTypeId = 1,
                Price = 50,
                TimeRequired = TimeSpan.FromHours(2)
            };

            // Act
            var result = await _controller.Create(model);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(1, await _context.WorkshopServices.CountAsync());
        }

        [Test]
        public async Task Create_Post_Should_Return_View_When_ModelInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Some error");
            var model = new WorkshopService();

            // Act
            var result = await _controller.Create(model);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Delete_Should_Remove_WorkshopService_When_Exists()
        {
            // Arrange
            var workshopService = new WorkshopService
            {
                WorkshopId = 2,
                ServiceTypeId = 2,
                Price = 80,
                TimeRequired = TimeSpan.FromMinutes(45)
            };
            await _context.WorkshopServices.AddAsync(workshopService);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(2, 2);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(0, await _context.WorkshopServices.CountAsync());
        }

        [Test]
        public async Task Delete_Should_Return_NotFound_When_NotExists()
        {
            // Act
            var result = await _controller.Delete(99, 99);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}