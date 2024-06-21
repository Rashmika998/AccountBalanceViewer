using AccountBalanceViewer.Data;
using AccountBalanceViewer.Models;
using AccountBalanceViewer.Services;
using Microsoft.EntityFrameworkCore;

namespace BalanceTest
{
    [TestClass]
    public class BalanceServiceTest
    {
        private DbContextOptions<DataContext> _options;
        private DataContext _context;
        private BalanceService _balanceService;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                        .UseInMemoryDatabase(databaseName: "TestDatabase")
                        .Options;
            _context = new DataContext(_options);
            _balanceService = new BalanceService(_context);

            // Seed the in-memory database with test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.AccountBalances.AddRange(
                new AccountBalance { RnD = 1000, Canteen = 500, CEOCarExpenses = 200, Marketing = 800, ParkingFines = 100 },
                new AccountBalance { RnD = 2000, Canteen = 700, CEOCarExpenses = 300, Marketing = 900, ParkingFines = 150 }
            );
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task GetTotalBalances_ShouldReturnCorrectSums()
        {
            // Act
            var result = await _balanceService.GetTotalBalances();

            // Assert
            Assert.AreEqual(3000, result.RnD);
            Assert.AreEqual(1200, result.Canteen);
            Assert.AreEqual(500, result.CEOCarExpenses);
            Assert.AreEqual(1700, result.Marketing);
            Assert.AreEqual(250, result.ParkingFines);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}