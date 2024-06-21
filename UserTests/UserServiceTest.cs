using System.Security.Claims;
using System.Text;
using AccountBalanceViewer.Data;
using AccountBalanceViewer.Models;
using AccountBalanceViewer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.IdentityModel.Tokens;

namespace UserTests
{
    [TestClass]
    public class UserServiceTest
    {
        [TestClass]
        public class UserServiceTests
        {
            private DbContextOptions<DataContext> _options;
            private DataContext _context;
            private Mock<IConfiguration> _configurationMock;
            private UserService _service;

            [TestInitialize]
            public void Setup()
            {
                _options = new DbContextOptionsBuilder<DataContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;
                _context = new DataContext(_options);

                _configurationMock = new Mock<IConfiguration>();
                _configurationMock.Setup(c => c["Jwt:Key"]).Returns("@MySecret123ForJwtToken11111111111111111111");
                _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
                _configurationMock.Setup(c => c["Jwt:TokenValidityInMinutes"]).Returns("60");

                _service = new UserService(_context, _configurationMock.Object);
            }

            [TestMethod]
            public async Task Register_ShouldCreateUser()
            {
                // Arrange
                var newUser = new User { Email = "test@example.com", Password = "Password123", UserRole = "User" };

                // Act
                var response = await _service.Register(newUser);

                // Assert
                Assert.AreEqual("User created successfully!", response.Message);
                Assert.IsNotNull(await _context.Users.FindAsync("test@example.com"));
            }

            [TestMethod]
            public async Task Register_ShouldReturnError_WhenUserExists()
            {
                // Arrange
                var existingUser = new User { Email = "test@example.com", Password = "Password123", UserRole = "User" };
                _context.Users.Add(existingUser);
                await _context.SaveChangesAsync();

                // Act
                var response = await _service.Register(existingUser);

                // Assert
                Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);
                Assert.AreEqual("User already exists!", response.Message);
            }

            [TestMethod]
            public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
            {
                // Arrange
                var passwordHasher = new PasswordHasher<object>();
                var hashedPassword = passwordHasher.HashPassword(null, "Password123");
                var existingUser = new User { Email = "test@example.com", Password = hashedPassword, UserRole = "User" };
                _context.Users.Add(existingUser);
                await _context.SaveChangesAsync();

                var loggedUser = new User { Email = "test@example.com", Password = "Password123", UserRole = "User" };

                // Act
                var response = await _service.Login(loggedUser);

                // Assert
                Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
                Assert.IsTrue(response.Message.Contains("VALIDTO"));
            }

            [TestMethod]
            public async Task Login_ShouldReturnError_WhenCredentialsAreInvalid()
            {
                // Arrange
                var loggedUser = new User { Email = "nonexistent@example.com", Password = "Password123", UserRole = "User" };

                // Act
                var response = await _service.Login(loggedUser);

                // Assert
                Assert.AreEqual(StatusCodes.Status400BadRequest, response.StatusCode);
                Assert.AreEqual("Invalid username or password.", response.Message);
            }

            [TestMethod]
            public async Task Logout_ShouldBlacklistToken()
            {
                // Arrange
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("@MySecret123ForJwtToken11111111111111111111");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, "test@example.com")
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = "TestIssuer",
                    Audience = "TestIssuer"
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // Act
                var response = await _service.Logout(tokenString);

                // Assert
                Assert.AreEqual(StatusCodes.Status200OK, response.StatusCode);
                Assert.AreEqual("User logged out successfully", response.Message);
                Assert.IsNotNull(await _context.BlacklistedTokens.FirstOrDefaultAsync(t => t.Token == tokenString));
            }

            [TestCleanup]
            public void Cleanup()
            {
                _context.Database.EnsureDeleted();
                _context.Dispose();
            }
        }
    }
}