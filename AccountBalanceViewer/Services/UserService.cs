using AccountBalanceViewer.Authentication;
using AccountBalanceViewer.Data;
using AccountBalanceViewer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountBalanceViewer.Services
{
    public class UserService:IUserService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Response> Register(User newUser)
        {
            var user = await _context.Users.FindAsync(newUser.Email);
            if (user != null)
                return new Response { StatusCode = StatusCodes.Status400BadRequest, Message = "User already exists!" };

            var passwordHasher = new PasswordHasher<object>();
            newUser.Password = passwordHasher.HashPassword(newUser, newUser.Password);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new Response { StatusCode = StatusCodes.Status400BadRequest, Message = "User created successfully!" };
        }

        public async Task<Response> Login(User loggedUser)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loggedUser.Email);
            if (user == null)
                return new Response { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid username or password." };

            if (user.UserRole != loggedUser.UserRole)
                return new Response { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid username or password." };

            var passwordHasher = new PasswordHasher<object>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, loggedUser.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
                return new Response { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid username or password." };
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.UserRole)
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                 issuer: _configuration["Jwt:Issuer"],
                 audience: _configuration["Jwt:Issuer"],
                 expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:TokenValidityInMinutes"])),
                 claims: authClaims,
                 signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
             );

            return new Response { StatusCode = StatusCodes.Status200OK, Message = new JwtSecurityTokenHandler().WriteToken(token)+"VALIDTO"+token.ValidTo };
        }

        public async Task<Response> Logout(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new Response { StatusCode = StatusCodes.Status400BadRequest, Message = "Token required." }; 
            }

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var expiryDate = jwtToken.ValidTo;

            var blacklistedToken = new BlacklistedToken
            {
                Token = token,
                ExpiryDate = expiryDate
            };

            _context.BlacklistedTokens.Add(blacklistedToken);
            await _context.SaveChangesAsync();

            return new Response { StatusCode = StatusCodes.Status200OK, Message = "User logged out successfully" };
        }
    }
}
