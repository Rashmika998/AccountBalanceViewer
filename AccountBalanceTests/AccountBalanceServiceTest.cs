using AccountBalanceViewer.Data;
using AccountBalanceViewer.Models;
using AccountBalanceViewer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountBalanceTests
{
    [TestClass]
    public class AccountBalanceServiceTests
    {
        private DbContextOptions<DataContext> _options;
        private DataContext _context;
        private AccountBalanceService _service;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                        .UseInMemoryDatabase(databaseName: "TestDatabase")
                        .Options;
            _context = new DataContext(_options);
            _service = new AccountBalanceService(_context);

            // Seed the in-memory database with test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.AccountBalances.AddRange(
                new AccountBalance { Id = 1, RnD = 1000, Canteen = 500, CEOCarExpenses = 200, Marketing = 800, ParkingFines = 100 },
                new AccountBalance { Id = 2, RnD = 2000, Canteen = 700, CEOCarExpenses = 300, Marketing = 900, ParkingFines = 150 }
            );
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task GetAccountBalances_ShouldReturnAllBalances()
        {
            // Act
            var result = await _service.GetAccountBalances();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<List<AccountBalance>>));
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(2, result.Value.Count);
        }

        [TestMethod]
        public async Task GetAccountBalance_ShouldReturnCorrectBalance()
        {
            // Act
            var result = await _service.GetAccountBalance(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<AccountBalance>));
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
        }

        [TestMethod]
        public async Task AddAccountBalance_ShouldAddBalance()
        {
            // Arrange
            var newBalance = new AccountBalance { Id = 3, RnD = 3000, Canteen = 800, CEOCarExpenses = 400, Marketing = 1000, ParkingFines = 200 };

            // Act
            var result = await _service.AddAccountBalance(newBalance);
            var balances = result.Value;

            // Assert
            Assert.IsNotNull(balances);
            Assert.AreEqual(3, balances.Count);
            Assert.IsTrue(balances.Exists(b => b.Id == 3));
        }

        [TestMethod]
        public async Task UpdateAccountBalance_ShouldUpdateBalance()
        {
            // Arrange
            var updatedBalance = new AccountBalance { Id = 1, RnD = 1500, Canteen = 600, CEOCarExpenses = 250, Marketing = 850, ParkingFines = 120 };

            // Act
            var result = await _service.UpdateAccountBalance(updatedBalance);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, result);
            var balance = await _context.AccountBalances.FindAsync(1);
            Assert.AreEqual(1500, balance.RnD);
        }

        [TestMethod]
        public async Task UpdateAccountBalance_ShouldReturnNotFound()
        {
            // Arrange
            var updatedBalance = new AccountBalance { Id = 999, RnD = 1500, Canteen = 600, CEOCarExpenses = 250, Marketing = 850, ParkingFines = 120 };

            // Act
            var result = await _service.UpdateAccountBalance(updatedBalance);

            // Assert
            Assert.AreEqual(StatusCodes.Status404NotFound, result);
        }

        [TestMethod]
        public async Task DeleteAccountBalance_ShouldDeleteBalance()
        {
            // Act
            var result = await _service.DeleteAccountBalance(1);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, result);
            var balance = await _context.AccountBalances.FindAsync(1);
            Assert.IsNull(balance);
        }

        [TestMethod]
        public async Task DeleteAccountBalance_ShouldReturnNotFound()
        {
            // Act
            var result = await _service.DeleteAccountBalance(999);

            // Assert
            Assert.AreEqual(StatusCodes.Status404NotFound, result);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}