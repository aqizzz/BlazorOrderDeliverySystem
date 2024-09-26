using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using static OrderDeliverySystem.Share.Data.Constants;
using System.Collections.Immutable;

namespace OrderDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(AppDbContext context, IConfiguration config) : ControllerBase
    {
        [HttpGet("{userId}")]
        //[Authorize]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            //var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //if (!User.IsInRole("Admin") && userId != loggedInUserId)
            //{
            //    return Forbid();
            //}

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Type == "Main");

            UserProfileDTO profile = new()
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

            return Ok(profile);
        }

        [HttpGet("worker/{userId}")]
        //[Authorize(Roles = "Admin, Worker")]
        public async Task<IActionResult> GetWorkerInfo(int userId)
        {
            //var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //if (!User.IsInRole("Admin") && userId != loggedInUserId)
            //{
            //    return Forbid();
            //}

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            var worker = await context.DeliveryWorkers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (worker == null)
                return NotFound("Worker not found");

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Type == "Main");

            WorkerProfileDTO profile = new()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                WorkerAvailability = worker.WorkerAvailability,
                CommissionRate= worker.CommissionRate,
                LastTaskAssigned = worker.LastTaskAssigned,
                Unit = address?.Unit ?? "",
                Address = address?.Address ?? "",
                City = address?.City ?? "",
                Province = address?.Province ?? "",
                Postcode = address?.Postcode ?? ""
            };

            return Ok(profile);
        }



        [HttpGet("merchants")]
        //[Authorize(Roles = "Admin, Merchant")]
        public async Task<ActionResult<IEnumerable<MerchantProfileDTO>>> GetMerchants()
        {
            var merchants = await context.Merchants.ToListAsync(); // Execute the query

            if (merchants == null || !merchants.Any()) // Check if the list is null or empty
            {
                return NotFound("Merchant not found");
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

                var profile = new MerchantProfileDTO
                {
                    UserId = merchant.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Email = user.Email,
                    BusinessName = merchant.BusinessName ?? "New Business",
                    MerchantPic = merchant.MerchantPic ?? "https://www.eclosio.ong/wp-content/uploads/2018/08/default.png",
                    MerchantDescription = merchant.MerchantDescription ?? "",
                    PreparingTime = merchant.PreparingTime ?? 0,
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

        [HttpGet("merchant/{userId}")]
        //[Authorize(Roles = "Admin, Merchant")]
        public async Task<IActionResult> GetMerchantInfo(int? userId)
        {
            //var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //if (!User.IsInRole("Admin") && userId != loggedInUserId)
            //{
            //    return Forbid();
            //}

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound("User not found");

            var merchant = await context.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
            if (merchant == null)
                return NotFound("Merchant not found");

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Type == "Main");

            MerchantProfileDTO profile = new MerchantProfileDTO()
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
                Unit = address?.Unit ?? "",
                Address = address?.Address ?? "",
                City = address?.City ?? "",
                Province = address?.Province ?? "",
                Postcode = address?.Postcode ?? ""
            };

            return Ok(profile);
        }

        [HttpPut("edit")]
        //[Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> Edit(UserProfileDTO dto)
        {
            //var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //if (!User.IsInRole("Admin") && dto.UserId != loggedInUserId)
            //{
            //    return Forbid();
            //}

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);

                var existingUserWithEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.UserId != dto.UserId);
                if (existingUserWithEmail != null)
                    return Conflict("Email already exists for another user");

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
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, "An error occurred while updating the profile");
            }
        }

        [HttpPut("edit/worker")]
        [Authorize(Roles = "Admin, Worker")]
        public async Task<IActionResult> Edit(WorkerProfileDTO dto)
        {
            //var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //if (!User.IsInRole("Admin") && dto.UserId != loggedInUserId)
            //{
            //    return Forbid();
            //}

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
                if (user == null)
                    return NotFound("User not found");

                var existingUserWithEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.UserId != dto.UserId);
                if (existingUserWithEmail != null)
                    return Conflict("Email already exists for another user");

                var worker = await context.DeliveryWorkers.FirstOrDefaultAsync(d => d.UserId == dto.UserId);
                if (worker == null)
                    return NotFound("Delivery worker not found");

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
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, "An error occurred while updating the profile");
            }
        }

        [HttpPut("edit/merchant")]
        //[Authorize(Roles = "Admin, Merchant")]
        public async Task<IActionResult> Edit(MerchantProfileDTO dto)
        {
            //var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //if (!User.IsInRole("Admin") && dto.UserId != loggedInUserId)
            //{
            //    return Forbid();
            //}

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
                if (user == null)
                    return NotFound("User not found");

                var merchant = await context.Merchants.FirstOrDefaultAsync(m => m.UserId == dto.UserId);
                if (merchant == null)
                    return NotFound("Merchant not found");

                var existingUserWithEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.UserId != dto.UserId);
                if (existingUserWithEmail != null)
                    return Conflict("Email already exists for another user");

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
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, "An error occurred while updating the profile");
            }
        }
    }
}