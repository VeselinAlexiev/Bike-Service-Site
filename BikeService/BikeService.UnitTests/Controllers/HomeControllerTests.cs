using BikeService.Controllers;
using BikeService.Data;
using BikeService.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private ApplicationDbContext _context;
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("HomeTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            var loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(loggerMock.Object, _context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void Index_Should_Return_View_With_Data()
        {
            _context.Bicycles.AddRange(
    new Bicycle
    {
        Id = 1,
        Model = "X1",
        Brand = "Trek",
        Price = 1000,
        ImageUrl = "bike1.jpg",
        EnergySource = "Manual",
        Material = "Steel",
        Type = "Road",
        Description = "desc"
    },
    new Bicycle
    {
        Id = 2,
        Model = "X2",
        Brand = "Scott",
        Price = 900,
        ImageUrl = "bike2.jpg",
        EnergySource = "Manual",
        Material = "Steel",
        Type = "Road",
        Description = "desc"
    }
);


            _context.SpareParts.AddRange(
                new SparePart { Id = 1, Name = "Chain", Price = 30, ImageUrl = "part1.jpg", Description = "desc", Compatibility = "All" },
                new SparePart { Id = 2, Name = "Brake", Price = 40, ImageUrl = "part2.jpg", Description = "desc", Compatibility = "All" }
            );

            _context.SaveChanges();

            var result = _controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}