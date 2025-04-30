using BikeService.Controllers;
using BikeService.Data;
using BikeService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class AdminSparePartControllerTests
    {
        private ApplicationDbContext _context;
        private AdminSparePartController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new AdminSparePartController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_Should_Return_View_With_SpareParts()
        {
            // Arrange
            await _context.SpareParts.AddAsync(new SparePart { Name = "Chain", Description = "Bike chain", Price = 20, Compatibility = "MTB", StockQuantity = 5, ImageUrl = "img.jpg", EcoFriendly = true });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task CreatePost_Should_Add_SparePart()
        {
            // Arrange
            var form = new FormCollection(new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "Name", "Brake" },
                { "Description", "Brake set" },
                { "Price", "50" },
                { "Compatibility", "MTB" },
                { "StockQuantity", "10" },
                { "ImageUrl", "brake.jpg" },
                { "EcoFriendly", "true" }
            });
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.ControllerContext.HttpContext.Request.Form = form;

            // Act
            var result = await _controller.CreatePost();

            // Assert
            Assert.AreEqual(1, await _context.SpareParts.CountAsync());
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task EditPost_Should_Update_SparePart()
        {
            // Arrange
            var part = new SparePart { Name = "Seat", Description = "Bike seat", Price = 30, Compatibility = "Road", StockQuantity = 2, ImageUrl = "seat.jpg", EcoFriendly = false };
            await _context.SpareParts.AddAsync(part);
            await _context.SaveChangesAsync();

            var form = new FormCollection(new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "Name", "Updated Seat" },
                { "Description", "Updated bike seat" },
                { "Price", "40" },
                { "Compatibility", "Road" },
                { "StockQuantity", "4" },
                { "ImageUrl", "updatedseat.jpg" },
                { "EcoFriendly", "true" }
            });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.ControllerContext.HttpContext.Request.Form = form;

            // Act
            var result = await _controller.EditPost(part.Id);

            // Assert
            var updated = await _context.SpareParts.FirstAsync();
            Assert.AreEqual("Updated Seat", updated.Name);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Delete_Should_Remove_SparePart()
        {
            // Arrange
            var part = new SparePart { Name = "Tire", Description = "Bike tire", Price = 60, Compatibility = "MTB", StockQuantity = 6, ImageUrl = "tire.jpg", EcoFriendly = true };
            await _context.SpareParts.AddAsync(part);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(part.Id);

            // Assert
            var exists = await _context.SpareParts.FindAsync(part.Id);
            Assert.IsNull(exists);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
    }
}