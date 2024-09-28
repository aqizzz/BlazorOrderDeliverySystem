using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.Data.Models;
using static OrderDeliverySystem.Share.Data.Constants.Merchant;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderDeliverySystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AppDbContext context, IConfiguration config) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(CustomerRegisterDTO user)
        {
            if (user == null)
                return BadRequest("User data is required.");

            using var transaction = await context.Database.BeginTransactionAsync();

            var getUser = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (getUser != null)
                return BadRequest("User already exists");

            var newUser = new User
            {
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.Now,
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            var newCustomer = new Customer
            {
                User = newUser,
                UserId = newUser.UserId
            };

            context.Customers.Add(newCustomer);
            await context.SaveChangesAsync();

            var newAddress = new AddressModel
            {
                User = newUser,
                UserId = newUser.UserId,
                Type = "Main",
                Unit = "",
                Address = "",
                City = "",
                Province = "",
                Postcode = ""
            };
            context.Addresses.Add(newAddress);
            await context.SaveChangesAsync(); 

            await transaction.CommitAsync();

            return Ok("Success");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest("Email and password are required");

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
                return BadRequest("Invalid credentials");

            var verifyPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!verifyPassword)
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user.UserId, user.Email, user.Role);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(int userId, string email, string role)
        {
            // Retrieve the JWT settings from the configuration
            var key = config["Jwt:Key"];
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];
            var expiresInMinutesString = config["Jwt:ExpiresInMinutes"];

            // Check if any JWT settings are missing or invalid
            if (string.IsNullOrWhiteSpace(key) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) ||
                !int.TryParse(expiresInMinutesString, out var expiresInMinutes))
            {
                throw new InvalidOperationException("JWT settings are not properly configured.");
            }

            var jwtSettings = new JwtSettings
            {
                Key = key,
                Issuer = issuer,
                Audience = audience,
                ExpiresInMinutes = expiresInMinutes
            };

            var symmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key));
            var creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiresInMinutes),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto passwordDto)
        {
            var userClaims = User;
            var userId = Convert.ToInt32(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            var verifyPassword = BCrypt.Net.BCrypt.Verify(passwordDto.Password, user.PasswordHash);
            if (!verifyPassword)
                return BadRequest("Current password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordDto.NewPassword);
            await context.SaveChangesAsync();

            return Ok("Password has been successfully changed");
        }


        [HttpPost("register/worker")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(WorkerRegisterDTO user)
        {
            if (user == null)
                return BadRequest("User data is required.");

            using var transaction = await context.Database.BeginTransactionAsync();

            var getUser = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (getUser != null)
                return BadRequest("User already exists");

            var newUser = new User
            {
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Phone = user.Phone ?? "",
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Role = "Worker",
                IsActive = true,
                CreatedAt = DateTime.Now,
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            var newWorker = new DeliveryWorker
            {
                User = newUser,
                UserId = newUser.UserId,
                WorkerAvailability = false,
                CommissionRate = user.CommissionRate,
                LastTaskAssigned = null
            };

            context.DeliveryWorkers.Add(newWorker);

            var newAddress = new AddressModel
            {
                User = newUser,
                UserId = newUser.UserId,
                Type = "Main",
                Unit = user.Unit ?? "",
                Address = user.Address ?? "",
                City = user.City ?? "",
                Province = user.Province ?? "",
                Postcode = user.Postcode ?? ""
            };
            context.Addresses.Add(newAddress);
            await context.SaveChangesAsync(); 

            await transaction.CommitAsync();

            return Ok("Success");
        }

        [HttpPost("register/merchant")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(MerchantRegisterDTO user)
        {
            if (user == null)
                return BadRequest("User data is required.");

            using var transaction = await context.Database.BeginTransactionAsync();

            var getUser = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (getUser != null)
                return BadRequest("User already exists");

            var newUser = new User
            {
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Phone = user.Phone ?? "",
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Role = "Merchant",
                IsActive = true,
                CreatedAt = DateTime.Now,
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync(); 

            var newMerchant = new Merchant
            {
                User = newUser,
                UserId = newUser.UserId,
                BusinessName = user.BusinessName ?? "",
                MerchantPic = user.MerchantPic ?? DefaultPic,
                MerchantDescription = user.MerchantDescription ?? "",
                PreparingTime = user.PreparingTime ?? 0
            };

            context.Merchants.Add(newMerchant);

            var newAddress = new AddressModel
            {
                User = newUser,
                UserId = newUser.UserId,
                Type = "Main",
                Unit = user.Unit ?? "",
                Address = user.Address ?? "",
                City = user.City ?? "",
                Province = user.Province ?? "",
                Postcode = user.Postcode ?? ""
            };
            context.Addresses.Add(newAddress);

            await context.SaveChangesAsync(); 

            await transaction.CommitAsync();

            return Ok("Success");
        }

    }
 
}
