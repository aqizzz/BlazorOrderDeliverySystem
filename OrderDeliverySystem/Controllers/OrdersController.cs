using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.Data.Models;

namespace OrderDeliverySystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(AppDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public OrdersController(AppDbContext context)
        : this(context, null) // Providing a default value for ILogger (null in this case)
        {
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Merchant)
                .Include(o => o.DeliveryWorker)
                .Include(o => o.PickupAddress)
                .Include(o => o.DropoffAddress)
                .Include(o => o.OrderItems)
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }


        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDTO orderDto)
        {
            // Fetch required entities from the database (Merchant and Customer)
            var merchant = await _context.Merchants.FindAsync(orderDto.MerchantId);
            if (merchant == null)
            {
                return NotFound($"Merchant with ID {orderDto.MerchantId} not found.");
            }

            var customer = await _context.Customers.FindAsync(orderDto.CustomerId);
            if (customer == null)
            {
                return NotFound($"Customer with ID {orderDto.CustomerId} not found.");
            }

            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                MerchantId = orderDto.MerchantId,
                WorkerId = orderDto.WorkerId,
                PickupAddressId = orderDto.PickupAddressId,
                DropoffAddressId = orderDto.DropoffAddressId,
                TotalAmount = orderDto.TotalAmount,
                Status = orderDto.Status,
                CreatedAt = orderDto.CreatedAt,
                UpdatedAt = orderDto.UpdatedAt,
                Merchant = merchant,
                Customer = customer,
                

            };

            order.OrderItems = new List<OrderItem>();
            foreach (var orderItemDto in orderDto.OrderItems)
            {
                // Fetch the Item based on ItemId from the DTO
                var item = await _context.Items.FindAsync(orderItemDto.ItemId);
                if (item == null)
                {
                    return NotFound($"Item with ID {orderItemDto.ItemId} not found.");
                }

                var orderItem = new OrderItem
                {
                    ItemId = orderItemDto.ItemId,
                    Quantity = orderItemDto.Quantity,
                    Order = order, // Set the Order property
                    Item = item    // Set the Item property (required)
                };

                order.OrderItems.Add(orderItem);
            }



            // Add order to context
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Return created order
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order updatedOrder)
        {
            if (id != updatedOrder.OrderId)
            {
                return BadRequest();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Update order fields
            order.CustomerId = updatedOrder.CustomerId;
            order.MerchantId = updatedOrder.MerchantId;
            order.WorkerId = updatedOrder.WorkerId;
            order.PickupAddressId = updatedOrder.PickupAddressId;
            order.DropoffAddressId = updatedOrder.DropoffAddressId;
            order.TotalAmount = updatedOrder.TotalAmount;
            order.Status = updatedOrder.Status;
            order.UpdatedAt = DateTime.Now;

            // Update the order in the context
            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to check if an order exists
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(o => o.OrderId == id);
        }
    }
}
