using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Services
{
    [TestFixture]
    public class CartServiceTests
    {
        private ApplicationDbContext _context;
        private ICartService _cartService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _cartService = new CartService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddToCartAsync_Should_Add_SparePart_To_Cart()
        {
            // Arrange
            var userId = "user1";
            var sparePart = new SparePart
            {
                Id = 1,
                Name = "Brake",
                Price = 50,
                Compatibility = "Mountain Bike",
                Description = "High quality brake for mountain bikes",
                ImageUrl = "https://example.com/brake.jpg"
            };
            await _context.SpareParts.AddAsync(sparePart);
            await _context.SaveChangesAsync();

            // Act
            await _cartService.AddToCartAsync(userId, sparePart.Id, 2);

            // Assert
            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            Assert.IsNotNull(cart);
            Assert.AreEqual(1, cart.Items.Count);
            Assert.AreEqual(sparePart.Id, cart.Items.First().PartId);
            Assert.AreEqual(2, cart.Items.First().Quantity);
        }

        [Test]
        public async Task RemoveFromCartAsync_Should_Remove_Item()
        {
            // Arrange
            var userId = "user2";
            var cart = new Cart { UserId = userId };
            cart.Items.Add(new CartItem { PartId = 2, Price = 20, Quantity = 1 });
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Act
            await _cartService.RemoveFromCartAsync(userId, 2);

            // Assert
            var updatedCart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            Assert.IsEmpty(updatedCart.Items);
        }

        [Test]
        public async Task ChangeQuantityAsync_Should_Increase_Quantity()
        {
            // Arrange
            var userId = "user3";
            var cart = new Cart { UserId = userId };
            cart.Items.Add(new CartItem { PartId = 3, Price = 30, Quantity = 1 });
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Act
            await _cartService.ChangeQuantityAsync(userId, 3, 2);

            // Assert
            var updatedCart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            Assert.AreEqual(3, updatedCart.Items.First().Quantity);
        }
        [Test]
        public async Task GetCartItemsAsync_Should_Return_Items_When_Cart_Exists()
        {
            // Arrange
            var userId = "user4";
            var cart = new Cart { UserId = userId };
            cart.Items.Add(new CartItem
            {
                PartId = 1,
                Price = 100,
                Quantity = 2,
                SparePart = new SparePart
                {
                    Id = 1,
                    Name = "Handlebar",
                    Description = "Mountain handlebar",
                    Compatibility = "Mountain",
                    ImageUrl = "https://example.com/handlebar.jpg",
                    Price = 100
                }
            });
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartService.GetCartItemsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Handlebar", result.First().Name);
        }

        [Test]
        public async Task ChangeQuantityAsync_Should_Remove_Item_When_Quantity_Goes_To_Zero()
        {
            // Arrange
            var userId = "user5";
            var cart = new Cart { UserId = userId };
            cart.Items.Add(new CartItem { PartId = 2, Price = 50, Quantity = 1 });
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Act
            await _cartService.ChangeQuantityAsync(userId, 2, -1);

            // Assert
            var updatedCart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            Assert.IsEmpty(updatedCart.Items);
        }
    }
}
