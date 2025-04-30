using BikeService.Data;
using BikeService.Data.Entities;
using BikeService.Services.Implementation;
using BikeService.Web.ViewModel.Order;
using Microsoft.EntityFrameworkCore;
using Moq;
using NETCore.MailKit.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private ApplicationDbContext _context;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Mocked email service but NOT used
            var emailServiceMock = new Mock<IEmailService>();
            _orderService = new OrderService(_context, emailServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateOrderAsync_Should_CreateOrder_When_CartIsNotEmpty()
        {
            var userId = "user1";
            var sparePart = new SparePart
            {
                Id = 1,
                Name = "Wheel",
                Price = 100,
                Compatibility = "Universal",
                Description = "Standard wheel",
                ImageUrl = "https://example.com/wheel.jpg"
            };
            var cart = new Cart
            {
                UserId = userId,
                Items = new List<CartItem> { new CartItem { PartId = 1, Quantity = 2, Price = 100, SparePart = sparePart } }
            };
            var user = new User { Id = userId, Email = "user@mail.com", UserName = "TestUser" };

            await _context.SpareParts.AddAsync(sparePart);
            await _context.Carts.AddAsync(cart);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var checkout = new CheckoutViewModel { Address = "Test", Phone = "123456789", PaymentMethod = "Card" };

            var orderId = await _orderService.CreateOrderAsync(userId, checkout);

            var createdOrder = await _context.Orders.Include(o => o.OrderSpareParts).FirstOrDefaultAsync(o => o.Id == orderId);

            Assert.IsNotNull(createdOrder);
            Assert.AreEqual(1, createdOrder.OrderSpareParts.Count);
        }

        [Test]
        public void CreateOrderAsync_Should_Throw_When_CartIsEmpty()
        {
            var userId = "user2";
            _context.Carts.Add(new Cart { UserId = userId });
            _context.SaveChanges();

            var checkout = new CheckoutViewModel();

            Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.CreateOrderAsync(userId, checkout));
        }

        [Test]
        public async Task GetUserOrdersAsync_Should_ReturnOrders()
        {
            var userId = "user3";
            _context.Orders.Add(new Order { UserId = userId, TotalAmount = 100, Status = "Pending" });
            await _context.SaveChangesAsync();

            var orders = await _orderService.GetUserOrdersAsync(userId);

            Assert.AreEqual(1, orders.Count);
        }

        [Test]
        public async Task GetAllOrdersFilteredAsync_Should_Filter_By_Email()
        {
            var user = new User { Id = "user4", Email = "filter@mail.com", UserName = "FilterUser" };
            _context.Users.Add(user);
            _context.Orders.Add(new Order { UserId = user.Id, TotalAmount = 100, Status = "Pending", User = user });
            await _context.SaveChangesAsync();

            var filter = new OrderFilterViewModel { SearchEmail = "filter@mail.com", CurrentPage = 1, PageSize = 10 };
            var orders = await _orderService.GetAllOrdersFilteredAsync(filter);

            Assert.AreEqual(1, orders.Count);
        }

        [Test]
        public async Task CountOrdersAsync_Should_ReturnCorrectCount()
        {
            var user = new User { Id = "user5", Email = "count@mail.com", UserName = "CountUser" };
            _context.Users.Add(user);
            _context.Orders.Add(new Order { UserId = user.Id, TotalAmount = 100, Status = "Pending", User = user });
            await _context.SaveChangesAsync();

            var filter = new OrderFilterViewModel { SearchEmail = "count@mail.com" };
            var count = await _orderService.CountOrdersAsync(filter);

            Assert.AreEqual(1, count);
        }

        [Test]
        public async Task GetOrderDetailsAsync_Should_ReturnOrder_WhenExists()
        {
            var user = new User { Id = "user6", Email = "user6@mail.com", UserName = "U6" };
            var sparePart = new SparePart
            {
                Id = 1,
                Name = "Chain",
                Price = 20,
                Compatibility = "Universal",
                Description = "Chain desc",
                ImageUrl = "https://example.com/chain.jpg"
            };
            var order = new Order
            {
                Id = 1,
                UserId = user.Id,
                User = user,
                OrderSpareParts = new List<OrderSparePart>
    {
        new OrderSparePart { PartId = sparePart.Id, SparePart = sparePart, Quantity = 1 }
    }
            };

            _context.Users.Add(user);
            _context.SpareParts.Add(sparePart);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();


            var result = await _orderService.GetOrderDetailsAsync(order.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(order.Id, result.Id);
        }

        [Test]
        public void GetOrderDetailsAsync_Should_Throw_WhenNotFound()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.GetOrderDetailsAsync(999));
        }

        [Test]
        public async Task ChangeOrderStatusAsync_Should_UpdateStatus()
        {
            var user = new User { Id = "user7", Email = "change@mail.com", UserName = "ChangeUser" };
            var order = new Order { UserId = user.Id, Status = "Pending", User = user };
            _context.Users.Add(user);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _orderService.ChangeOrderStatusAsync(order.Id, "Completed");

            var updatedOrder = await _context.Orders.FirstOrDefaultAsync(o => o.Id == order.Id);
            Assert.AreEqual("Completed", updatedOrder.Status);
        }
    }
}