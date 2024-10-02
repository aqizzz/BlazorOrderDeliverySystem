using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OrderDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(AppDbContext context, IConfiguration config) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var profile = await GetUserProfile(userId);
            if (profile == null)
                return NotFound(new { Error = "User not found" });

            return Ok(profile);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var profile = await GetUserProfile(userId);
            if (profile == null)
                return NotFound(new { Error = "User not found" });

            return Ok(profile);
        }

        [HttpGet("worker")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> GetWorkerInfo()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var profile = await GetWorkerProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new { Error = "Worker not found" });
            }

            return Ok(profile);
        }

        [HttpGet("worker/availability")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> GetWorkerAvailability()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var worker = await context.DeliveryWorkers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (worker == null) return NotFound(new { Error = "Worker not found" });

            return Ok(worker.WorkerAvailability);
        }

        [HttpGet("worker/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetWorkerInfo(int userId)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var profile = await GetWorkerProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new { Error = "Worker not found" });
            }

            return Ok(profile);
        }

        [HttpGet("merchant")]
        public async Task<IActionResult> GetMerchantInfo()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var profile = await GetMerchantProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new { Error = "Merchant not found" });
            }

            return Ok(profile);
        }

        [HttpGet("merchant/{userId}")]
        public async Task<IActionResult> GetMerchantInfo(int userId)
        {
            var profile = await GetMerchantProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new { Error = "Merchant not found" });
            }

            return Ok(profile);
        }

        [HttpGet("merchant/item/{itemId}")]
        public async Task<IActionResult> GetMerchantInfoByItemId(int itemId)
        {
            var item = await context.Items
                .Include(i => i.Merchant) 
                .ThenInclude(m => m.User)
                .ThenInclude(u => u.Addresses)
                .FirstOrDefaultAsync(i => i.ItemId == itemId);

            if (item == null)
            {
                return NotFound(new { Error = "Merchant not found" });
            }

            var profile =  new MerchantProfileDTO
            {
                UserId = item.Merchant.UserId,
                FirstName = item.Merchant.User.FirstName,
                LastName = item.Merchant.User.LastName,
                Phone = item.Merchant.User.Phone,
                Email = item.Merchant.User.Email,
                BusinessName = item.Merchant.BusinessName ?? "New Business",
                MerchantPic = item.Merchant.MerchantPic ?? "https://www.eclosio.ong/wp-content/uploads/2018/08/default.png",
                MerchantDescription = item.Merchant.MerchantDescription ?? "",
                PreparingTime = item.Merchant.PreparingTime ?? 0,
                Type = "Main",
                Unit = item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.Unit ?? "",
                Address = item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.Address ?? "",
                City = item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.City ?? "",
                Province = item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.Province ?? "",
                Postcode = item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.Postcode ?? ""
            };

            return Ok(profile);
        }

        [HttpPut("edit")]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> Edit([FromBody] CustomerProfileDTO dto)
        {
            var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!User.IsInRole("Admin") && dto.UserId != loggedInUserId)
            {
                return Forbid();
            }

            if (dto == null)
            {
                return BadRequest(new { Error = "Profile data is required." });
            }

            using var transaction = await context.Database.BeginTransactionAsync();

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
            if (user == null)
            {
                return NotFound(new { Error = "User not found." });
            }

            var existingUserWithEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.UserId != dto.UserId);
            if (existingUserWithEmail != null)
                return Conflict(new { Error = "Email already exists for another user" });

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == dto.UserId && a.Type == "Main");
            if (address == null)
            {
                address = new AddressModel { User = user, UserId = dto.UserId, Type = "Main" };
                context.Addresses.Add(address); // Add the new Address entity to the context
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Phone = dto.Phone;
            user.Email = dto.Email;

            address.Unit = dto.Unit;
            address.Address = dto.Address;
            address.City = dto.City;
            address.Province = dto.Province;
            address.Postcode = dto.Postcode;

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok("Profile has been successfully updated");
        }

        [HttpPut("edit/worker")]
        [Authorize(Roles = "Admin, Worker")]
        public async Task<IActionResult> Edit(WorkerProfileDTO dto)
        {
            var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!User.IsInRole("Admin") && dto.UserId != loggedInUserId)
            {
                return Forbid();

            }

            if (dto == null)
            {
                return BadRequest(new { Error = "Profile data is required." });
            }


            using var transaction = await context.Database.BeginTransactionAsync();

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
            if (user == null)
                return NotFound(new { Error = "User not found" });

            var existingUserWithEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.UserId != dto.UserId);
            if (existingUserWithEmail != null)
                return Conflict(new { Error = "Email already exists for another user" });

            var worker = await context.DeliveryWorkers.FirstOrDefaultAsync(d => d.UserId == dto.UserId);
            if (worker == null)
                return NotFound(new { Error = "Delivery worker not found" });

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == dto.UserId && a.Type == "Main");
            if (address == null)
            {
                address = new AddressModel { User = user, UserId = dto.UserId, Type = "Main" };
                context.Addresses.Add(address); // Add the new Address entity to the context
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Phone = dto.Phone;
            user.Email = dto.Email;

            worker.CommissionRate = dto.CommissionRate;

            address.Unit = dto.Unit;
            address.Address = dto.Address;
            address.City = dto.City;
            address.Province = dto.Province;
            address.Postcode = dto.Postcode;

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok("Profile has been successfully updated");
        }

        [HttpPut("worker/availability")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> EditWorkerAvailability([FromBody] AvailabilityUpdateDTO model)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var worker = await context.DeliveryWorkers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (worker == null) return NotFound(new { Error = "Worker not found" });

            worker.WorkerAvailability = model.Availability;
            await context.SaveChangesAsync();  

            return Ok(new { Success = true });
        }

        [HttpPut("edit/merchant")]
        [Authorize(Roles = "Admin, Merchant")]
        public async Task<IActionResult> Edit(MerchantProfileDTO dto)
        {
            var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!User.IsInRole("Admin") && dto.UserId != loggedInUserId)
            {
                return Forbid();
            }

            if (dto == null)
            {
                return BadRequest(new { Error = "Profile data is required." });
            }


            using var transaction = await context.Database.BeginTransactionAsync();

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
            if (user == null)
                return NotFound(new { Error = "User not found" });

            var merchant = await context.Merchants.FirstOrDefaultAsync(m => m.UserId == dto.UserId);
            if (merchant == null)
                return NotFound(new { Error = "Merchant not found" });

            var existingUserWithEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.UserId != dto.UserId);
            if (existingUserWithEmail != null)
                return Conflict(new { Error = "Email already exists for another user" });

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == dto.UserId && a.Type == "Main");
            if (address == null)
            {
                address = new AddressModel { User = user, UserId = dto.UserId, Type = "Main" };
                context.Addresses.Add(address); // Add the new Address entity to the context
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Phone = dto.Phone;
            user.Email = dto.Email;

            merchant.BusinessName = dto.BusinessName;
            merchant.MerchantPic = dto.MerchantPic;
            merchant.MerchantDescription = dto.MerchantDescription;
            merchant.PreparingTime = dto.PreparingTime;

            address.Unit = dto.Unit;
            address.Address = dto.Address;
            address.City = dto.City;
            address.Province = dto.Province;
            address.Postcode = dto.Postcode;

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok("Profile has been successfully updated");
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserProfileDTO>>> GetUsers()
        {
            var users = await context.Users
                .Where(u => u.Role != "Admin")
                .ToListAsync();

            if (users == null || !users.Any()) // Check if the list is null or empty
            {
                return NotFound(new { Error = "User not found" });
            }


            var results = new List<UserProfileDTO>();
            var userIds = users.Select(m => m.UserId).ToList();
            var addresses = await context.Addresses
                .Where(a => userIds.Contains(a.UserId) && a.Type == "Main")
                .ToListAsync();

            foreach (var user in users)
            {
                var address = addresses.FirstOrDefault(a => a.UserId == user.UserId);

                var profile = new UserProfileDTO
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    Email = user.Email ?? "",
                    Phone = user.Phone ?? "",
                    Role = user.Role,
                    Type = "Main",
                    Unit = address?.Unit ?? "",
                    Address = address?.Address ?? "",
                    City = address?.City ?? "",
                    Province = address?.Province ?? "",
                    Postcode = address?.Postcode ?? ""
                };

                results.Add(profile);
            }
            return Ok(results);
        }

        [HttpGet("merchants")]
        //[Authorize(Roles = "Admin, Merchant")]
        public async Task<ActionResult<IEnumerable<MerchantProfileDTO>>> GetMerchants()
        {
            var merchants = await context.Merchants.ToListAsync(); // Execute the query

            if (merchants == null || !merchants.Any()) // Check if the list is null or empty
            {
                return NotFound(new { Error = "Merchant not found" });
            }


            var results = new List<MerchantProfileDTO>();
            var userIds = merchants.Select(m => m.UserId).ToList();
            var addresses = await context.Addresses
                .Where(a => userIds.Contains(a.UserId))
                .ToListAsync();

            var users = await context.Users
               .Where(a => userIds.Contains(a.UserId))
               .ToListAsync();

            foreach (var merchant in merchants)
            {
                var user = users.FirstOrDefault(a => a.UserId == merchant.UserId);
                var address = addresses.FirstOrDefault(a => a.UserId == merchant.UserId);

                var profile = await GetMerchantProfileAsync(merchant.UserId);

                results.Add(profile);
            }
            return Ok(results);
        }
        private async Task<CustomerProfileDTO?> GetUserProfile(int userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Type == "Main");

            return new CustomerProfileDTO
            {
                UserId = user.UserId,
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Email = user.Email ?? "",
                Phone = user.Phone ?? "",
                Type = "Main",
                Unit = address?.Unit ?? "",
                Address = address?.Address ?? "",
                City = address?.City ?? "",
                Province = address?.Province ?? "",
                Postcode = address?.Postcode ?? ""
            };
        }
        private async Task<WorkerProfileDTO?> GetWorkerProfileAsync(int userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;

            var worker = await context.DeliveryWorkers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (worker == null) return null;

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Type == "Main");

            return new WorkerProfileDTO
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                WorkerAvailability = worker.WorkerAvailability,
                CommissionRate = worker.CommissionRate,
                LastTaskAssigned = worker.LastTaskAssigned,
                Type = "Main",
                Unit = address?.Unit ?? "",
                Address = address?.Address ?? "",
                City = address?.City ?? "",
                Province = address?.Province ?? "",
                Postcode = address?.Postcode ?? ""
            };
        }

        private async Task<MerchantProfileDTO?> GetMerchantProfileAsync(int userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;


            var merchant = await context.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
            if (merchant == null) return null;

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Type == "Main");

            return new MerchantProfileDTO
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                BusinessName = merchant.BusinessName ?? "New Business",
                MerchantPic = merchant.MerchantPic ?? "https://www.eclosio.ong/wp-content/uploads/2018/08/default.png",
                MerchantDescription = merchant.MerchantDescription ?? "",
                PreparingTime = merchant.PreparingTime ?? 0,
                Type = "Main",
                Unit = address?.Unit ?? "",
                Address = address?.Address ?? "",
                City = address?.City ?? "",
                Province = address?.Province ?? "",
                Postcode = address?.Postcode ?? ""
            };
        }
    }
}