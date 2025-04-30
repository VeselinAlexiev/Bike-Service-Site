using BikeService.Controllers;
using BikeService.Data;
using BikeService.Data.Entities;
using BikeService.Web.ViewModel.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class ServiceControllerTests
    {
        private ApplicationDbContext _context;
        private ServiceController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ServiceTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new ServiceController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_Should_Return_View_With_Services()
        {
            // Arrange
            var serviceType = new ServiceType
            {
                Id = 1,
                Title = "Inspection",
                Description = "Full check"
            };

            var workshop = new Workshop
            {
                Id = 1,
                Name = "BikeFix",
                Location = "Sofia",
                Latitude = 42.0,
                Longitude = 23.0,
                WorkshopServices = new List<WorkshopService>
                {
                    new WorkshopService
                    {
                        ServiceType = serviceType,
                        Price = 25,
                        TimeRequired = System.TimeSpan.FromMinutes(30)
                    }
                }
            };

            _context.ServiceTypes.Add(serviceType);
            _context.Workshops.Add(workshop);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<List<ServiceViewModel>>(viewResult.Model);
            Assert.AreEqual(1, ((List<ServiceViewModel>)viewResult.Model).Count);
        }

        [Test]
        public async Task Details_Should_Return_View_When_Workshop_Exists()
        {
            // Arrange
            var serviceType = new ServiceType
            {
                Id = 2,
                Title = "Repair",
                Description = "Minor repair"
            };

            var workshop = new Workshop
            {
                Id = 2,
                Name = "FixPro",
                Location = "Plovdiv",
                Latitude = 42.2,
                Longitude = 24.7,
                WorkshopServices = new List<WorkshopService>
                {
                    new WorkshopService
                    {
                        ServiceType = serviceType,
                        Price = 40,
                        TimeRequired = System.TimeSpan.FromMinutes(45)
                    }
                }
            };

            var bicycle = new Bicycle
            {
                Id = 1,
                Brand = "Trek",
                Model = "Marlin",
                Description = "Reliable mountain bike",
                Year = 2023,
                Type = "Mountain",
                ImageUrl = "https://example.com/bike.jpg",
                EnergySource = "Manual",
                Material = "Aluminum"
            };


            _context.ServiceTypes.Add(serviceType);
            _context.Workshops.Add(workshop);
            _context.Bicycles.Add(bicycle);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(2);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<ServiceDetailsViewModel>(viewResult.Model);
        }

        [Test]
        public async Task Details_Should_Return_NotFound_When_Workshop_Not_Found()
        {
            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}