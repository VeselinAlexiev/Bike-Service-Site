using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class ShopControllerTests
    {
        private Mock<IShopService> _shopServiceMock;
        private Mock<ICartService> _cartServiceMock;
        private ShopController _shopController;

        [SetUp]
        public void Setup()
        {
            _shopServiceMock = new Mock<IShopService>();
            _cartServiceMock = new Mock<ICartService>();
            _shopController = new ShopController(_shopServiceMock.Object, _cartServiceMock.Object);
        }

        [Test]
        public async Task Details_ShouldReturnView_WhenItemExists()
        {
            // Arrange
            var itemId = 1;
            _shopServiceMock.Setup(x => x.GetItemByIdAsync(itemId))
                            .ReturnsAsync(new ProductDetailsViewModel { Id = itemId });

            // Act
            var result = await _shopController.Details(itemId);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Details_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            _shopServiceMock.Setup(x => x.GetItemByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync((ProductDetailsViewModel)null);

            // Act
            var result = await _shopController.Details(999);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task RemoveFromCart_Should_Redirect_To_Cart()
        {
            // Arrange
            var userId = "test-user";
            var controller = new ShopController(_shopServiceMock.Object, _cartServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "TestAuth"))
                }
            };

            // Act
            var result = await controller.RemoveFromCart(1);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Cart", redirectResult.ActionName);
        }

        [Test]
        public async Task IncreaseQuantity_Should_Redirect_To_Cart()
        {
            // Arrange
            var userId = "test-user2";
            var controller = new ShopController(_shopServiceMock.Object, _cartServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "TestAuth"))
                }
            };

            // Act
            var result = await controller.IncreaseQuantity(1);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Cart", redirectResult.ActionName);
        }
    }
}
