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
    public class AdminBicycleControllerTests
    {
        private ApplicationDbContext _context;
        private AdminBicycleController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("BicycleDb_" + System.Guid.NewGuid())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new AdminBicycleController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_Should_Return_View_With_Bicycles()
        {
            await _context.Bicycles.AddAsync(new Bicycle
            {
                Model = "TestBike",
                Brand = "BrandX",
                Type = "MTB",
                Description = "Desc",
                ImageUrl = "img.jpg",
                EnergySource = "Manual",
                Material = "Aluminum"
            });
            await _context.SaveChangesAsync();

            var result = await _controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<System.Collections.Generic.List<Bicycle>>(viewResult.Model);
        }

        [Test]
        public void Create_Should_Return_View()
        {
            var result = _controller.Create();
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task EditGet_Should_Return_View_When_Bike_Exists()
        {
            var bike = new Bicycle
            {
                Model = "ModelX",
                Brand = "BrandY",
                Type = "City",
                Description = "Good bike",
                ImageUrl = "img2.jpg",
                EnergySource = "Manual",
                Material = "Steel"
            };

            await _context.Bicycles.AddAsync(bike);
            await _context.SaveChangesAsync();

            var result = await _controller.Edit(bike.Id);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task EditGet_Should_Return_NotFound_When_Bike_NotExists()
        {
            var result = await _controller.Edit(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_Should_Update_And_Redirect_When_Bike_Exists()
        {
            var bike = new Bicycle
            {
                Model = "OldModel",
                Brand = "OldBrand",
                Type = "Road",
                Description = "Old desc",
                ImageUrl = "old.jpg",
                EnergySource = "Manual",
                Material = "Aluminum"
            };
            await _context.Bicycles.AddAsync(bike);
            await _context.SaveChangesAsync();

            var httpContext = new DefaultHttpContext();
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "Model", "NewModel" },
                { "Brand", "NewBrand" },
                { "Year", "2022" },
                { "Type", "Electric" },
                { "Price", "1234.56" },
                { "Description", "Updated description" },
                { "ImageUrl", "new.jpg" },
                { "Material", "Carbon" },
                { "BatteryCapacity", "500" },
                { "EnergySource", "Electric" },
                { "EcoFriendly", "true" }
            });
            httpContext.Request.Form = form;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await _controller.EditPost(bike.Id);

            var updatedBike = await _context.Bicycles.FindAsync(bike.Id);

            Assert.IsNotNull(updatedBike);
            Assert.AreEqual("NewModel", updatedBike.Model);
            Assert.AreEqual(2022, updatedBike.Year);
            Assert.AreEqual("Electric", updatedBike.Type);
            Assert.AreEqual(1234.56m, updatedBike.Price);
            Assert.AreEqual("Updated description", updatedBike.Description);
            Assert.AreEqual("Carbon", updatedBike.Material);
            Assert.AreEqual(500, updatedBike.BatteryCapacity);
            Assert.AreEqual("Electric", updatedBike.EnergySource);
            Assert.IsTrue(updatedBike.EcoFriendly);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task EditPost_Should_Return_NotFound_When_Bike_NotExists()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = await _controller.EditPost(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Should_Remove_Bike_And_Redirect()
        {
            var bike = new Bicycle
            {
                Model = "ToDelete",
                Brand = "DeleteBrand",
                Type = "City",
                Description = "To delete",
                ImageUrl = "del.jpg",
                EnergySource = "Manual",
                Material = "Steel"
            };
            await _context.Bicycles.AddAsync(bike);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(bike.Id);

            var deletedBike = await _context.Bicycles.FindAsync(bike.Id);

            Assert.IsNull(deletedBike);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Delete_Should_Return_NotFound_When_Bike_NotExists()
        {
            var result = await _controller.Delete(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}