using BikeService.Controllers;
using BikeService.Data;
using BikeService.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class AdminWorkshopControllerTests
    {
        private ApplicationDbContext _context;
        private AdminWorkshopController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new AdminWorkshopController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_Should_Return_View_With_Workshops()
        {
            await _context.Workshops.AddAsync(new Workshop { Name = "TestW", Location = "Loc", Latitude = 0, Longitude = 0 });
            await _context.SaveChangesAsync();

            var result = await _controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
            var view = result as ViewResult;
            Assert.IsInstanceOf<List<Workshop>>(view.Model);
        }

        [Test]
        public async Task CreatePost_Should_Add_Workshop_And_Redirect()
        {
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "Name", "New Workshop" },
                { "Location", "Sofia" },
                { "Latitude", "42.7" },
                { "Longitude", "23.3" }
            });

            var context = new DefaultHttpContext();
            var formFeature = new FormFeature(formCollection);
            context.Features.Set<IFormFeature>(formFeature);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            var result = await _controller.CreatePost();

            Assert.AreEqual(1, await _context.Workshops.CountAsync());
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Edit_Should_Return_View_With_Workshop()
        {
            var workshop = new Workshop { Id = 10, Name = "W", Location = "L", Latitude = 1, Longitude = 1 };
            await _context.Workshops.AddAsync(workshop);
            await _context.SaveChangesAsync();

            var result = await _controller.Edit(10);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task EditPost_Should_Update_Workshop_And_Redirect()
        {
            var workshop = new Workshop { Id = 15, Name = "Old", Location = "Old", Latitude = 0, Longitude = 0 };
            await _context.Workshops.AddAsync(workshop);
            await _context.SaveChangesAsync();

            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "Name", "Updated" },
                { "Location", "NewLoc" },
                { "Latitude", "1.1" },
                { "Longitude", "2.2" }
            });

            var context = new DefaultHttpContext();
            context.Features.Set<IFormFeature>(new FormFeature(form));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            var result = await _controller.EditPost(15);
            var updated = await _context.Workshops.FindAsync(15);

            Assert.AreEqual("Updated", updated.Name);
            Assert.AreEqual("NewLoc", updated.Location);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Delete_Should_Remove_Workshop_And_Redirect()
        {
            var workshop = new Workshop { Id = 21, Name = "ToDelete", Location = "Loc", Latitude = 0, Longitude = 0 };
            await _context.Workshops.AddAsync(workshop);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(21);

            var exists = await _context.Workshops.FindAsync(21);
            Assert.IsNull(exists);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
    }
}