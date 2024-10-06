using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using System.Security.Claims;
using OrderDeliverySystem.Share.Data.Models;

namespace OrderDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController(AppDbContext context, IConfiguration config) : ControllerBase
    {
        [HttpGet("list")]
        [Authorize]
        public async Task<IActionResult> GetAddressList()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var addresses = await context.Addresses.Where(a => a.UserId == userId)
                                  .Select(a => new AddressCreateDTO
                                  {
                                      AddressId = a.AddressId,
                                      Type = a.Type,
                                      Unit = a.Unit,
                                      Address = a.Address,
                                      City = a.City,
                                      Province = a.Province,
                                      Postcode = a.Postcode,
                                      ContactName = a.ContactName,
                                      Phone = a.Phone,
                                  })
                                  .ToListAsync();

            if (addresses == null)
                return NotFound(new { Error = "Address list not found" });

            return Ok(addresses);
        }

        [HttpGet("{addressId}")]
        [Authorize]
        public async Task<IActionResult> GetAddress(int addressId)
        {
            var address = await context.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId);

            if (address== null)
                return NotFound(new { Error = "Address not found" });

            var addressDto = new AddressCreateDTO
            {
                AddressId = addressId,
                Type = address.Type,
                Unit = address.Unit,
                Address = address.Address,
                City = address.City,
                Province = address.Province,
                Postcode = address.Postcode,
                ContactName = address.ContactName,
                Phone = address.Phone,
            };

            return Ok(addressDto);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAddress(AddressCreateDTO addressDto)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await context.Users.FindAsync(userId);
            if (user == null) return NotFound(new { Error = "User not found" });

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Type == addressDto.Type);

            if (address == null)
                return NotFound(new { Error = "Address not found" });
            
            address.Unit = addressDto.Unit;
            address.Address= addressDto.Address;
            address.City = addressDto.City;
            address.Postcode = addressDto.Postcode;
            address.Province = addressDto.Province;
            address.ContactName = addressDto.ContactName;
            address.Phone = addressDto.Phone;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAddress(AddressCreateDTO addressDto)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await context.Users.FindAsync(userId);
            if (user == null) return NotFound(new { Error = "User not found" });

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Type == addressDto.Type);

            if (address != null)
                return Conflict(new { Error = "Address type already exists" });

            address = new AddressModel
            {
                UserId = userId,
                Type = addressDto.Type,
                Unit = addressDto.Unit,
                Address = addressDto.Address,
                City = addressDto.City,
                Postcode = addressDto.Postcode,
                Province = addressDto.Province,
                ContactName = addressDto.ContactName,
                Phone = addressDto.Phone,
                User = user
            };

            context.Addresses.Add(address);
            await context.SaveChangesAsync();

            return Ok();
        }


    }
}
