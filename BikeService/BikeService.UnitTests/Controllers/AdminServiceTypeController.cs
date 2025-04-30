using BikeService.Controllers;
using BikeService.Data;
using BikeService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class AdminServiceTypeControllerTests
    {
        private ApplicationDbContext _context;
        private AdminServiceTypeController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ServiceTypeDb_" + System.Guid.NewGuid())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new AdminServiceTypeController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_Should_Return_View_With_ServiceTypes()
        {
            await _context.ServiceTypes.AddAsync(new ServiceType { Title = "Test Service", Description = "Test Description" });
            await _context.SaveChangesAsync();

            var result = await _controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<System.Collections.Generic.List<ServiceType>>(viewResult.Model);
        }

        [Test]
        public async Task CreatePost_Should_Add_ServiceType_And_Redirect()
        {
            var service = new ServiceType { Title = "Oil Change", Description = "Change engine oil" };

            var result = await _controller.Create(service);

            Assert.That(_context.ServiceTypes.CountAsync().Result, Is.EqualTo(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task CreatePost_Should_Return_View_When_ModelState_Invalid()
        {
            _controller.ModelState.AddModelError("Title", "Required");
            var service = new ServiceType { Description = "No Title" };

            var result = await _controller.Create(service);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task EditGet_Should_Return_View_When_ServiceType_Exists()
        {
            var service = new ServiceType { Title = "Brake Repair", Description = "Fix brakes" };
            await _context.ServiceTypes.AddAsync(service);
            await _context.SaveChangesAsync();

            var result = await _controller.Edit(service.Id);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task EditGet_Should_Return_NotFound_When_ServiceType_Not_Exists()
        {
            var result = await _controller.Edit(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_Should_Update_ServiceType_And_Redirect()
        {
            var service = new ServiceType { Title = "Old", Description = "Old desc" };
            await _context.ServiceTypes.AddAsync(service);
            await _context.SaveChangesAsync();

            var updatedService = new ServiceType { Title = "New", Description = "New desc" };

            var result = await _controller.Edit(service.Id, updatedService);

            var entity = await _context.ServiceTypes.FindAsync(service.Id);
            Assert.AreEqual("New", entity.Title);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task EditPost_Should_Return_View_When_ModelState_Invalid()
        {
            _controller.ModelState.AddModelError("Title", "Required");
            var service = new ServiceType { Title = "", Description = "" };

            var result = await _controller.Edit(1, service);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task EditPost_Should_Return_NotFound_When_ServiceType_Not_Exists()
        {
            var service = new ServiceType { Title = "NonExistent", Description = "Desc" };

            var result = await _controller.Edit(999, service);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Should_Remove_ServiceType_And_Redirect()
        {
            var service = new ServiceType { Title = "Temp Service", Description = "Temp" };
            await _context.ServiceTypes.AddAsync(service);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(service.Id);

            Assert.That(await _context.ServiceTypes.CountAsync(), Is.EqualTo(0));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Delete_Should_Return_NotFound_When_ServiceType_Not_Exists()
        {
            var result = await _controller.Delete(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}