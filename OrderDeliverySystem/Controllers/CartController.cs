﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs.CartDTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace OrderDeliverySystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }



        // GET: api/cart/getCart/{customerId}
        [HttpGet("getCart")]
        public async Task<IActionResult> GetCartItems()
        {

            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);


            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            int customerId = customer.CustomerId;

            var cart = await _context.Carts
                .Include(c => c.Customer)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {

                cart = new Cart
                {
                    CustomerId = customerId,
                    Customer = customer,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }


            var cartDto = new GetCartReponseDTO(
                cart.CartId,
                cart.CustomerId,
                cart.CartItems?.Select(ci => new GetCartItemsResponseDTO(
                    ci.CartItemId,
                    ci.ItemId,
                    ci.Item.ItemName ?? "Unknown Item",
                    ci.Item.ItemPrice,
                    ci.Item.ItemPic ?? "",
                    ci.Quantity
                )).ToList() ?? new List<GetCartItemsResponseDTO>()
            );

            return Ok(cartDto);
        }




        // POST: api/cart/addCart/{customerId}
        [HttpPost("addCart")]
        public async Task<IActionResult> AddCartItems([FromBody] List<AddUpdateCartItemsRequestDTO> cartItemsDto)
        {

            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }


            int customerId = customer.CustomerId;


            var cart = await _context.Carts
                .Include(c => c.Customer)
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {

                cart = new Cart
                {
                    CustomerId = customerId,
                    Customer = customer,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }


            foreach (var itemDto in cartItemsDto)
            {
                var item = await _context.Items.FindAsync(itemDto.ItemId);
                if (item == null)
                {
                    return BadRequest($"Item with ID {itemDto.ItemId} not found.");
                }


                var existingCartItem = cart.CartItems?.FirstOrDefault(ci => ci.ItemId == itemDto.ItemId);

                if (existingCartItem != null)
                {

                    if (itemDto.Quantity <= 0)
                    {
                        cart.CartItems?.Remove(existingCartItem);
                    }
                    else
                    {
                        existingCartItem.Quantity += itemDto.Quantity;
                    }
                }
                else
                {

                    if (itemDto.Quantity > 0)
                    {
                        var cartItem = new CartItem
                        {
                            ItemId = itemDto.ItemId,
                            Quantity = itemDto.Quantity,
                            Cart = cart,
                            Item = item
                        };
                        cart.CartItems?.Add(cartItem);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Cart added successfully.");
        }



        // PUT: api/cart/updateCart/{customerId}
        [HttpPut("updateCart")]
        public async Task<IActionResult> UpdateCartItem([FromBody] AddUpdateCartItemsRequestDTO cartItemDto)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            int customerId = customer.CustomerId;

            var cart = await _context.Carts.Include(c => c.CartItems)
                                           .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                return NotFound("Cart not found for the customer.");

            var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.ItemId == cartItemDto.ItemId);

            if (cartItem == null)
                return NotFound($"Item with ID {cartItemDto.ItemId} not found in the cart.");


            if (cartItemDto.Quantity <= 0)
            {

                cart.CartItems?.Remove(cartItem);
            }
            else
            {

                cartItem.Quantity = cartItemDto.Quantity;
            }

            await _context.SaveChangesAsync();
            return Ok("Cart item updated.");
        }


        // DELETE: api/cart/clearCart/{customerId}
        [HttpDelete("clearCart")]
        public async Task<IActionResult> ClearCartItems()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);


            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            int customerId = customer.CustomerId;
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                return NotFound("Cart not found for the customer.");

            if (cart.CartItems == null || !cart.CartItems.Any())
            {
                return Ok("Cart is already empty.");
            }

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return Ok("Cart cleared.");
        }


        // DELETE: api/cart/deleteItem/{customerId}/{itemId}
        [HttpDelete("deleteItem/{itemId}")]
        public async Task<IActionResult> DeleteCartItem(int itemId)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);


            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            int customerId = customer.CustomerId;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                return NotFound("Cart not found for the customer.");


            var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.ItemId == itemId);

            if (cartItem == null)
                return NotFound($"Item with ID {itemId} not found in the cart.");


            cart.CartItems?.Remove(cartItem);


            await _context.SaveChangesAsync();

            return Ok($"Item with ID {itemId} removed from the cart.");
        }


        // GET: api/cart/totalQuantity
        [HttpGet("totalQuantity")]
        public async Task<IActionResult> GetTotalCartQuantity()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            int customerId = customer.CustomerId;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return Ok(0); // Return 0 if no cart exists or it's empty
            }

            // Calculate total quantity of items in the cart
            int totalQuantity = cart.CartItems.Sum(ci => ci.Quantity);

            return Ok(totalQuantity);
        }



    }
}
