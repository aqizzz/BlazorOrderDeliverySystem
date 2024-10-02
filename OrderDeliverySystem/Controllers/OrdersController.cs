﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO.OrderDeliverySystem.Share.DTOs.CartDTO;
using static MudBlazor.CategoryTypes;
using System.Runtime.InteropServices;



namespace OrderDeliverySystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(AppDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }
        /* public OrdersController(AppDbContext context)
         : this(context, null) // Providing a default value for ILogger (null in this case)
         {
         }*/
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


        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> CreateOrder(CreateOrderDTO orderDto)
        {


            // Fetch required entities from the database (Merchant and Customer)
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


            var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.UserId == userId);


            if (customer == null)
            {
                return NotFound("Customer not found.");
            }
            var customerId = orderDto.CustomerId;
            if (customerId != customer.CustomerId)
            {
                return NotFound("Not the user");
            }

            var worker = await _context.DeliveryWorkers
                .Where(w => w.WorkerAvailability == true) // Ensure worker is available and has a task history
                .OrderBy(w => w.LastTaskAssigned) // Get the worker with the oldest LastTaskAssigned date
                .FirstOrDefaultAsync();
            if (worker == null)
            {
                return NotFound($"No worker was found.");
            }
            var customerAddress = await _context.Addresses
              .Where(a => a.UserId == customer.UserId)
              .FirstOrDefaultAsync(); // ToListAsync if it's a collection

            if (customerAddress == null)
            {
                return NotFound($"Customer with ID {customer.UserId} not found.");
            }


            var merchants = orderDto.Merchants;
            if (merchants == null)
            {
                return NotFound($"No merchant was found.");
            }
            if (merchants.Count() > 0) {
                foreach (var thisMerchant in merchants)
                {
                    if (thisMerchant == null)
                    {
                        return NotFound($"No merchant was found.");
                    }


                    var merchant = await _context.Merchants.FindAsync(thisMerchant.UserId);
                    if (merchant == null)
                    {
                        return NotFound($"Merchant with ID {thisMerchant.UserId} not found.");
                    }

                    var merchantAddress = await _context.Addresses
                       .Where(a => a.UserId == merchant.UserId)
                       .FirstOrDefaultAsync();

                    if (merchantAddress == null)
                    {
                        return NotFound($"Address with ID {merchant.UserId} not found.");
                    }



                    var order = new Order
                    {
                        CustomerId = customer.CustomerId,
                        MerchantId = merchant.MerchantId,
                        PickupAddressId = merchantAddress.AddressId,
                        DropoffAddressId = customerAddress.AddressId,
                        TotalAmount = 0.00m,
                        Status = "Pending",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Merchant = merchant,
                        Customer = customer,
                        OrderItems = new List<OrderItem>(),
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    var newOrderId = order.OrderId;
                    decimal totalAmount = 0m;
                    foreach (var orderItemDto in orderDto.CartItems)
                    {
                        // Fetch the Item based on ItemId from the DTO
                        var item = await _context.Items.FindAsync(orderItemDto.ItemId);
                        if (item == null)
                        {
                            return NotFound($"Item with ID {orderItemDto.ItemId} not found.");
                        }
                        if (item.MerchantId == merchant.MerchantId)
                        {
                            var orderItem = new OrderItem
                            {
                                ItemId = orderItemDto.ItemId,
                                Quantity = orderItemDto.Quantity,
                                PriceAtOrder = orderItemDto.ItemPrice,
                                OrderId = newOrderId,
                                Discount = 0.00m,
                                Tax = 0.15m,
                                Order = order, // Set the Order property
                                Item = item    // Set the Item property (required)
                            };

                            order.OrderItems.Add(orderItem);
                            totalAmount += (decimal)orderItemDto.ItemPrice * (decimal)orderItemDto.Quantity;
                        }
                    }

                    order.TotalAmount = totalAmount;

                    await _context.SaveChangesAsync();
                    totalAmount = 0m;
                }
            }



            // Return created order
            return Ok("MenuItem has been successfully updated");

        }

        [HttpPut("update")]

        public async Task<IActionResult> UpdateOrder(OrderDTO updatedOrder)
        {


            var order = await _context.Orders.FindAsync(updatedOrder.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            // Update order fields

            order.Status = updatedOrder.Status;
            order.UpdatedAt = DateTime.Now;

            // Update the order in the context
            _context.Entry(order).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            var newTracking = new DeliveryTask
            {
                Order = order,
                OrderId = order.OrderId,
                AssignedTime = DateTime.Now,
                WorkerId = order.DeliveryWorker.WorkerId,
                DeliveryWorker = order.DeliveryWorker,
                Status = updatedOrder.Status,
            };
            _context.DeliveryTasks.Add(newTracking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("recent-orders/{customerId}")]
        public async Task<IActionResult> GetRecentOrdersByCustomer(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }
            var recentOrders = await _context.Orders
                .Where(o => o.CustomerId == customerId && o.Status != "Delivered")
                .Include(o => o.Customer)
                .Include(o => o.Merchant)
                .Include(o => o.DeliveryWorker)
                .Include(o => o.PickupAddress)
                .Include(o => o.DropoffAddress)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            if (!recentOrders.Any())
            {
                return NotFound("No recent orders found for this customer.");
            }
            return Ok(recentOrders);
        }

        [HttpGet("order-history/{customerId}")]
        public async Task<IActionResult> GetOrderHistoryByCustomer(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }
            var orderHistory = await _context.Orders
                .Where(o => o.CustomerId == customerId && o.Status == "Delivered" || o.Status == "Cancelled")
                .Include(o => o.Customer)
                .Include(o => o.Merchant)
                .Include(o => o.DeliveryWorker)
                .Include(o => o.PickupAddress)
                .Include(o => o.DropoffAddress)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            if (!orderHistory.Any())
            {
                return NotFound("No order history found for this customer.");
            }
            return Ok(orderHistory);
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

        [HttpGet("get-{role}")]
 
        public async Task<IActionResult> GetOrdersByRole(string role, bool recent = false)
        {
            // Fetch required entities from the database (Merchant and Customer)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized("User is not authenticated.");
            }

           
            if (!int.TryParse(userIdClaim, out int userId))
            {
                // Return bad request if the user ID is not valid
                return BadRequest("Invalid user ID.");
            }

            IQueryable<Order> query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Include(o => o.Merchant)
                .Include(o => o.DeliveryWorker) ;

         

            switch (role.ToLower())
            {
                case "customer":
                   
                    query = query.Where(o => o.Customer != null && o.Customer.UserId == userId); break;
                case "merchant":
                    query = query.Where(o => o.Merchant != null && o.Merchant.UserId == userId); break;
                case "worker":
                    query = query.Where(o => o.DeliveryWorker !=null && o.DeliveryWorker.UserId == userId); break;
            }

            if (recent)
            {
                query = query.Where(o => o.Status != "Delivered");
            }
            else
            {
                query = query.Where(o => o.Status == "Delivered" || o.Status == "Cancelled");
            }

            //Old
            /*var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    MerchantId = o.MerchantId,
                    WorkerId = o.WorkerId,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    OrderItems = o.OrderItems.Select(oi => new AppOrderItem
                    {
                        OrderItemId = oi.OrderItemId,
                        ItemId = oi.ItemId,
                        OrderId = oi.OrderId,
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .ToListAsync();*/

            //New
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    MerchantId = o.MerchantId,
                    WorkerId = o.WorkerId,
                    PickupAddressId = o.PickupAddressId,
                    DropoffAddressId = o.DropoffAddressId,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    Customer = new CustomerDTO1
                    {
                        CustomerId = o.Customer.CustomerId,
                        User = new UserDTO1
                        {
                            UserId = o.Customer.User.UserId,
                            FirstName = o.Customer.User.FirstName,
                            LastName = o.Customer.User.LastName,
                            Email = o.Customer.User.Email,
                            Phone = o.Customer.User.Phone,
                            Role = o.Customer.User.Role,
                            Addresses = o.Customer.User.Addresses.Select(a => new AddressModelDTO1
                            {
                                Type = a.Type,
                                Unit = a.Unit,
                                Address = a.Address,
                                City = a.City,
                                Province = a.Province,
                                Postcode = a.Postcode,
                            }).ToList()
                        }
                    },
                    Merchant = new MerchantDTO1
                    {
                        MerchantId = o.Merchant.MerchantId,
                        BusinessName = o.Merchant.BusinessName,
                        MerchantPic = o.Merchant.MerchantPic,
                        MerchantDescription = o.Merchant.MerchantDescription,
                        PreparingTime = o.Merchant.PreparingTime,
                        User = new UserDTO1
                        {
                            UserId = o.Merchant.User.UserId,
                            FirstName = o.Merchant.User.FirstName,
                            LastName = o.Merchant.User.LastName,
                            Email = o.Merchant.User.Email,
                            Phone = o.Merchant.User.Phone,
                            Role = o.Merchant.User.Role,
                            Addresses = o.Merchant.User.Addresses.Select(a => new AddressModelDTO1
                            {
                                Type = a.Type,
                                Unit = a.Unit,
                                Address = a.Address,
                                City = a.City,
                                Province = a.Province,
                                Postcode = a.Postcode
                            }).ToList()
                        }
                    },
                    OrderItems = o.OrderItems.Select(oi => new AppOrderItem
                    {
                        OrderItemId = oi.OrderItemId,
                        ItemId = oi.ItemId,
                        ItemName = oi.Item.ItemName,
                        OrderId = oi.OrderId,
                        ItemPrice = oi.PriceAtOrder,
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .ToListAsync();

            /*if (!orders.Any())
            {
                return Ok(new { Orders = new List<OrderDTO>() });
            }*/
            return Ok(orders);

        }


        [HttpGet("table/{role}")]
        [Authorize(Roles = "Customer,Merchant,Worker")]
        public async Task<IActionResult> GetOrdersTableByRole(string role, bool recent, int pageNumber = 1, int pageSize = 10)
       {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized("User is not authenticated.");
            }


            if (!int.TryParse(userIdClaim, out int userId))
            {
                // Return bad request if the user ID is not valid
                return BadRequest("Invalid user ID.");
            }


            IQueryable<Order> query = _context.Orders
              .Include(o => o.Customer)
              .Include(o => o.OrderItems)
                  .ThenInclude(oi => oi.Item)
              .Include(o => o.Merchant)
              .Include(o => o.DeliveryWorker);


            switch (role.ToLower())
            {
                case "customer":
             
                    query = query.Where(o => o.Customer.UserId == userId); break;
                case "merchant":
                    query = query.Where(o => o.Merchant.UserId == userId); break;
                case "worker":
                    query = query.Where(o => o.DeliveryWorker.UserId == userId); break;
            }

            if (recent)
            {
                query = query.Where(o => o.Status != "Delivered");
            }
            else
            {
                query = query.Where(o => o.Status == "Delivered" || o.Status == "Cancelled");
            }


            // 将 Item 实体转换为 ViewItemDTO
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    MerchantId = o.MerchantId,
                    WorkerId = o.WorkerId,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    Customer = new CustomerDTO1
                    {
                        CustomerId = o.Customer.CustomerId,
                        User = new UserDTO1
                        {
                            UserId = o.Customer.User.UserId,
                            FirstName = o.Customer.User.FirstName,
                            LastName = o.Customer.User.LastName,
                            Email = o.Customer.User.Email,
                            Phone = o.Customer.User.Phone,
                            Role = o.Customer.User.Role,
                            Addresses = o.Customer.User.Addresses.Select(a => new AddressModelDTO1
                            {
                                Type = a.Type,
                                Unit = a.Unit,
                                Address = a.Address,
                                City = a.City,
                                Province = a.Province,
                                Postcode = a.Postcode,
                            }).ToList()
                        }
                    },
                    Merchant = new MerchantDTO1
                    {
                        MerchantId = o.Merchant.MerchantId,
                        BusinessName = o.Merchant.BusinessName,
                        MerchantPic = o.Merchant.MerchantPic,
                        MerchantDescription = o.Merchant.MerchantDescription,
                        PreparingTime = o.Merchant.PreparingTime,
                        User = new UserDTO1
                        {
                            UserId = o.Merchant.User.UserId,
                            FirstName = o.Merchant.User.FirstName,
                            LastName = o.Merchant.User.LastName,
                            Email = o.Merchant.User.Email,
                            Phone = o.Merchant.User.Phone,
                            Role = o.Merchant.User.Role,
                            Addresses = o.Merchant.User.Addresses.Select(a => new AddressModelDTO1
                            {
                                Type = a.Type,
                                Unit = a.Unit,
                                Address = a.Address,
                                City = a.City,
                                Province = a.Province,
                                Postcode = a.Postcode
                            }).ToList()
                        }
                    },
                    OrderItems = o.OrderItems.Select(oi => new AppOrderItem
                    {
                        OrderItemId = oi.OrderItemId,
                        ItemId = oi.ItemId,
                        ItemName = oi.Item.ItemName,
                        OrderId = oi.OrderId,
                        Quantity = oi.Quantity

                    }).ToList()
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
       
            return Ok(orders);
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
                .ThenInclude(i => i.Merchant)
                .ThenInclude(m => m.User)
                .ThenInclude(u => u.Addresses)
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


            var cartDto = new GetOrderResponseDTO(
                cart.CartId,
                cart.CustomerId,
                cart.CartItems?.Select(ci => new GetOrderItemResponseDTO(
                    ci.CartItemId,
                    ci.Item.MerchantId,
                    ci.ItemId,
                    ci.Item.ItemName ?? "Unknown Item",
                    ci.Item.ItemPrice,
                    ci.Item.ItemPic ?? "",
                    ci.Quantity,
                    new MerchantProfileDTO
                    {
                        UserId = ci.Item.Merchant.UserId,
                        FirstName = ci.Item.Merchant.User.FirstName,
                        LastName = ci.Item.Merchant.User.LastName,
                        Phone = ci.Item.Merchant.User.Phone,
                        Email = ci.Item.Merchant.User.Email,
                        BusinessName = ci.Item.Merchant.BusinessName ?? "New Business",
                        MerchantPic = ci.Item.Merchant.MerchantPic ?? "https://www.eclosio.ong/wp-content/uploads/2018/08/default.png",
                        MerchantDescription = ci.Item.Merchant.MerchantDescription ?? "",
                        PreparingTime = ci.Item.Merchant.PreparingTime ?? 0,
                        Type = "Main",
                        Unit = ci.Item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.Unit ?? "",
                        Address = ci.Item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.Address ?? "",
                        City = ci.Item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.City ?? "",
                        Province = ci.Item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.Province ?? "",
                        Postcode = ci.Item.Merchant.User.Addresses?.FirstOrDefault(a => a.Type == "Main")?.Postcode ?? ""
                    }

                )).ToList() ?? new List<GetOrderItemResponseDTO>()
            );

            return Ok(cartDto);
        }


  
       /* public async Task<IActionResult> GetMerchantsByItems(List<int> itemId)
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

            var profile = new MerchantProfileDTO
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
*/    }
}