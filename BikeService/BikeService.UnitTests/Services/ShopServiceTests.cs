using BikeService.Data;
using BikeService.Data.Entities;
using BikeService.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BikeService.UnitTests.Services
{
    [TestFixture]
    public class ShopServiceTests
    {
        private ApplicationDbContext _context;
        private ShopService _shopService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ShopServiceDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _shopService = new ShopService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllItemsAsync_Should_Return_Items()
        {
            var bicycle = new Bicycle
            {
                Id = 3,
                Brand = "Cube",
                Model = "Aim",
                Description = "Entry bike",
                Price = 400,
                EnergySource = "Manual",
                ImageUrl = "https://example.com/bike2.jpg",
                Material = "Steel",
                Type = "Cross"
            };
            var sparePart = new SparePart
            {
                Id = 2,
                Name = "Chain",
                Description = "Bike chain",
                Price = 50,
                Compatibility = "Universal",
                ImageUrl = "url",
                EcoFriendly = false,
                StockQuantity = 10
            };

            _context.Bicycles.Add(bicycle);
            _context.SpareParts.Add(sparePart);
            await _context.SaveChangesAsync();

            var items = await _shopService.GetAllItemsAsync();

            Assert.AreEqual(2, items.Count());
        }

        [Test]
        public async Task GetItemByIdAsync_Should_Return_Bicycle()
        {
            var bicycle = new Bicycle
            {
                Id = 3,
                Brand = "Cube",
                Model = "Aim",
                Description = "Entry-level MTB",
                Price = 400,
                EnergySource = "Manual",
                ImageUrl = "https://example.com/bike2.jpg",
                Material = "Steel",
                Type = "Cross"
            };

            _context.Bicycles.Add(bicycle);
            await _context.SaveChangesAsync();

            var item = await _shopService.GetItemByIdAsync(3);

            Assert.IsNotNull(item);
            Assert.AreEqual("Bicycle", item.Type);
        }

        [Test]
        public async Task GetItemByIdAsync_Should_Return_SparePart()
        {
            var sparePart = new SparePart
            {
                Id = 4,
                Name = "Brake",
                Description = "Disk brake",
                Price = 30,
                Compatibility = "Mountain",
                ImageUrl = "url",
                EcoFriendly = true,
                StockQuantity = 5
            };
            _context.SpareParts.Add(sparePart);
            await _context.SaveChangesAsync();

            var item = await _shopService.GetItemByIdAsync(4);

            Assert.IsNotNull(item);
            Assert.AreEqual("SparePart", item.Type);
        }

        [Test]
        public async Task GetItemByIdAsync_Should_Return_Null_When_NotFound()
        {
            var item = await _shopService.GetItemByIdAsync(999);
            Assert.IsNull(item);
        }
    }
}