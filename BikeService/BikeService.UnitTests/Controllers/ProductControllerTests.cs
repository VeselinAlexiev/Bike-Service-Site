using BikeService.Controllers;
using BikeService.Services.Infrastructure;
using BikeService.Web.ViewModel.Details;
using BikeService.Web.ViewModel.Shop;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Controllers
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<IShopService> _shopServiceMock;
        private Mock<ICartService> _cartServiceMock;
        private ProductController _controller;

        [SetUp]
        public void Setup()
        {
            _shopServiceMock = new Mock<IShopService>();
            _cartServiceMock = new Mock<ICartService>();
            _controller = new ProductController(_shopServiceMock.Object, _cartServiceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task Details_Should_Return_View_When_Item_Found()
        {
            var item = new ProductDetailsViewModel
            {
                Id = 1,
                Name = "Main",
                Description = "desc",
                ImageUrl = "url",
                Price = 100,
                Category = "Test",
                Type = "SparePart",
                Quantity = 0
            };

            var relatedItems = new List<ShopItemViewModel>
{
    new ShopItemViewModel
    {
        Id = 2,
        Name = "Related",
        Description = "desc",
        ImageUrl = "url",
        Price = 50,
        Category = "Test",
        Type = "SparePart"
    }
};

            _shopServiceMock
                .Setup(s => s.GetItemByIdAsync(1))
                .ReturnsAsync(item);

            _shopServiceMock
                .Setup(s => s.GetAllItemsAsync(null, null))
                .ReturnsAsync(relatedItems);

            var result = await _controller.Details(1);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Details_Should_Return_NotFound_When_Item_Missing()
        {
            _shopServiceMock.Setup(s => s.GetItemByIdAsync(1)).ReturnsAsync((ProductDetailsViewModel)null);

            var result = await _controller.Details(1);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task AddToCart_Should_Return_Unauthorized_If_User_Missing()
        {
            var controller = new ProductController(_shopServiceMock.Object, _cartServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var model = new ProductDetailsViewModel { Id = 1, Quantity = 1 };
            var result = await controller.AddToCart(model);

            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task AddToCart_Should_Redirect_When_Valid()
        {
            var model = new ProductDetailsViewModel { Id = 1, Quantity = 1 };

            var result = await _controller.AddToCart(model);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Cart", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task AddToCart_Should_Return_View_When_ModelState_Invalid()
        {
            _controller.ModelState.AddModelError("Quantity", "Required");

            var model = new ProductDetailsViewModel();
            var result = await _controller.AddToCart(model);

            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}