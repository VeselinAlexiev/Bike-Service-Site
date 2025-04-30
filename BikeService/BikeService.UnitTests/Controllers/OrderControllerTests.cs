using BikeService.Controllers;
using BikeService.Data.Entities;
using BikeService.Services.Infrastructure;
using BikeService.Web.ViewModel.Order;
using BikeService.Web.ViewModel.Shop;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class OrderControllerTests
    {
        private Mock<IOrderService> _orderServiceMock;
        private Mock<ICartService> _cartServiceMock;
        private OrderController _controller;

        [SetUp]
        public void Setup()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _cartServiceMock = new Mock<ICartService>();
            _controller = new OrderController(_orderServiceMock.Object, _cartServiceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "test-user")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _controller.TempData = new TempDataDictionary(
                _controller.ControllerContext.HttpContext,
                Mock.Of<ITempDataProvider>());
        }

        [Test]
        public async Task Checkout_ShouldReturnView_WhenCartNotEmpty()
        {
            // Arrange
            var cartItems = new List<ShopItemViewModel>
            {
                new ShopItemViewModel { Id = 1, Name = "Test Part", Price = 100, Quantity = 1 }
            };

            _cartServiceMock.Setup(c => c.GetCartItemsAsync(It.IsAny<string>())).ReturnsAsync(cartItems);


            // Act
            var result = await _controller.Checkout();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Checkout_ShouldRedirect_WhenCartIsEmpty()
        {
            // Arrange
            _cartServiceMock.Setup(c => c.GetCartItemsAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<ShopItemViewModel>());

            // Act
            var result = await _controller.Checkout();

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Shop", redirectResult.ControllerName);
        }

        [Test]
        public async Task ConfirmCheckout_ShouldRedirectToMyOrders_WhenModelValid()
        {
            // Arrange
            var model = new CheckoutViewModel { Address = "Test", Phone = "123", PaymentMethod = "Cash" };
            _orderServiceMock.Setup(o => o.CreateOrderAsync(It.IsAny<string>(), It.IsAny<CheckoutViewModel>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.ConfirmCheckout(model);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("MyOrders", redirectResult.ActionName);
        }

        [Test]
        public async Task MyOrders_ShouldReturnView_WithOrders()
        {
            // Arrange
            _orderServiceMock.Setup(o => o.GetUserOrdersAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<Order>());

            // Act
            var result = await _controller.MyOrders();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task AllOrders_ShouldReturnView_WithFilteredOrders()
        {
            // Arrange
            _orderServiceMock.Setup(o => o.GetAllOrdersFilteredAsync(It.IsAny<OrderFilterViewModel>()))
                .ReturnsAsync(new List<Order>());
            _orderServiceMock.Setup(o => o.CountOrdersAsync(It.IsAny<OrderFilterViewModel>()))
                .ReturnsAsync(0);

            // Act
            var result = await _controller.AllOrders(new OrderFilterViewModel());

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task ChangeStatus_ShouldRedirectToAllOrders()
        {
            _orderServiceMock.Setup(o => o.ChangeOrderStatusAsync(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
            // Act
            var result = await _controller.ChangeStatus(1, "Completed");

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("AllOrders", redirectResult.ActionName);
        }

        [Test]
        public async Task Details_ShouldReturnView_WhenOrderExists()
        {
            // Arrange
            _orderServiceMock.Setup(o => o.GetOrderDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Order { Id = 1 });

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task LoadMoreOrders_ShouldReturnPartialView_WhenOrdersExist()
        {
            // Arrange
            _orderServiceMock.Setup(o => o.GetAllOrdersFilteredAsync(It.IsAny<OrderFilterViewModel>()))
                .ReturnsAsync(new List<Order>
                {
                    new Order()
                });

            // Act
            var result = await _controller.LoadMoreOrders(2);

            // Assert
            Assert.IsInstanceOf<PartialViewResult>(result);
        }
    }
}