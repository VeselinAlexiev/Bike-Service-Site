using BikeService.Controllers;
using BikeService.Data;
using BikeService.Data.Entities;
using BikeService.Web.ViewModel.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class AdminUserControllerTests
    {
        private ApplicationDbContext _context;
        private AdminUserController _controller;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
            _context = new ApplicationDbContext(options);

            var userStore = new UserStore<User>(_context);
            _userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);

            var roleStore = new RoleStore<IdentityRole>(_context);
            _roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

            _controller = new AdminUserController(_userManager, _roleManager, _context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_Should_Return_View_With_Users()
        {
            var user = new User { UserName = "testuser", Email = "test@example.com" };
            await _userManager.CreateAsync(user, "Password123!");

            var result = await _controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task AddRole_Should_Return_BadRequest_For_AdminRole()
        {
            var user = new User { UserName = "admin1", Email = "admin1@example.com" };
            await _userManager.CreateAsync(user, "Password123!");

            var result = await _controller.AddRole(user.Id, "Admin", null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task RemoveRole_Should_Return_BadRequest_For_AdminRole()
        {
            var user = new User { UserName = "admin2", Email = "admin2@example.com" };
            await _userManager.CreateAsync(user, "Password123!");

            var result = await _controller.RemoveRole(user.Id, "Admin");

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Delete_Should_Return_BadRequest_If_User_Is_Admin()
        {
            var user = new User { UserName = "admin3", Email = "admin3@example.com" };
            await _userManager.CreateAsync(user, "Password123!");

            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _userManager.AddToRoleAsync(user, "Admin");

            var result = await _controller.Delete(user.Id);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}