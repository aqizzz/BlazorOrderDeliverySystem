using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO.OrderDeliverySystem.Share.DTOs.CartDTO;



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


        [HttpPost("create")]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDTO orderDto)
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



            var merchant = await _context.Merchants.FindAsync(orderDto.MerchantId);
            if (merchant == null)
            {
                return NotFound($"Merchant with ID {orderDto.Merchant.UserId} not found.");
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
                WorkerId = worker.WorkerId,
                PickupAddressId = merchantAddress.AddressId,
                DropoffAddressId = customerAddress.AddressId,
                TotalAmount = orderDto.TotalAmount,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Merchant = merchant,
                Customer = customer,


            };

            order.OrderItems = new List<OrderItem>();
            foreach (var orderItemDto in orderDto.CartItems)
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

        [HttpGet("{role}/{id}")]
        public async Task<IActionResult> GetOrdersByRole(string role, int id, bool recent = false)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Include(o => o.Merchant)
                .Include(o => o.DeliveryWorker);

            switch (role.ToLower())
            {
                case "customer":
                    query = query.Where(o => o.CustomerId == id); break;
                case "merchant":
                    query = query.Where(o => o.MerchantId == id); break;
                case "worker":
                    query = query.Where(o => o.WorkerId == id); break;
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
                .ToListAsync();

            /*if (!orders.Any())
            {
                return Ok(new { Orders = new List<OrderDTO>() });
            }*/
            return Ok(orders);

        }


        [HttpGet("table/{role}/{userId}")]
        public async Task<IActionResult> GetOrdersTableByRole(string role, int userId, bool recent, int pageNumber = 1, int pageSize = 10)
        {
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


        [HttpGet("getOrderByCart/{cartId}")]
        public async Task<IActionResult> GetOrderByCart(int cartId)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


            var user = await _context.Users.FirstOrDefaultAsync(c => c.UserId == userId);


            if (user == null)
            {
                return NotFound("Customer not found.");
            }

            var cart = await _context.Carts.FindAsync(cartId);

            Console.WriteLine("cartId is " + cartId);
            if (cart == null)
            {
                return BadRequest("No item in the cart");
            }
            var cartItems = cart.CartItems;
            if (cartItems == null)
            {
                return BadRequest("No item in the cart");
            }

            var items = cartItems.Select(c => c.Item).ToList();
            if (items == null)
            {
                return BadRequest("No item in the cart");
            }


            var item = items.FirstOrDefault();
            if (item == null)
            {
                return BadRequest("No item in the cart");
            }
            var merchantId = item.MerchantId;
            Console.WriteLine("cartId is " + item);
            if (merchantId <= 0)
            {
                return BadRequest("No item in the cart");
            }
            var merchant = await _context.Merchants.FindAsync(merchantId);
            if (merchant == null)
            {
                return BadRequest("No item in the cart");
            }
            var merchantUser = await _context.Users.FindAsync(merchant.UserId);
            if (merchantUser == null)
            {
                return BadRequest("No item in the cart");
            }

            var address = await _context.Addresses.FindAsync(merchant.UserId);
            if (address == null)
            {
                return BadRequest("No item in the cart");
            }

            var orderDto = await _context.Carts
                .Select(c => new CreateOrderDTO
                {
                    CartId = c.CartId,
                    CustomerId = c.CustomerId,
                    TotalAmount = 0, // This can be calculated based on CartItems if needed
                    Merchant = new MerchantProfileDTO
                    {
                        BusinessName = merchant.BusinessName ?? "",
                        MerchantPic = merchant.MerchantPic,
                        PreparingTime = merchant.PreparingTime,
                        MerchantDescription = merchant.MerchantDescription,
                        UserId = merchant.UserId,
                        FirstName = merchantUser.FirstName,
                        LastName = merchantUser.LastName,
                        Unit = address.Unit,
                        Address = address.Address,
                        City = address.City,
                        Province = address.Province,
                        Postcode = address.Postcode

                    },
                    CartItems = c.CartItems.Select(ci => new CreateItemDTO
                    {
                        MerchantId = ci.Item.MerchantId,
                        ItemId = ci.ItemId,
                        ItemName = ci.Item.ItemName ?? "Unknown Item",
                        ItemPrice = ci.Item.ItemPrice,
                        ItemPic = ci.Item.ItemPic ?? "",
                        Quantity = ci.Quantity
                    }).ToList() ?? new List<CreateItemDTO>()
                })
                .FirstOrDefaultAsync(); // Use async version for database query







            return Ok(orderDto);
        }
        /*  /// GET: api/cart/getCart/{customerId}
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
                  .ThenInclude(cust => cust.User)
                  .ThenInclude(ca => ca.Addresses)
                  .Include(c => c.CartItems)
                  .ThenInclude(ci => ci.Item)
                  .FirstOrDefaultAsync(c => c.CustomerId == customerId);

              if (cart == null)
              {

                  return NotFound("Customer not found.");
              }
              var cartItems = cart.CartItems;
              if (cartItems == null)
              {

                  return NotFound("Items not found.");
              }
              var itemDto = cartItems.Select(i => new CreateItemDTO
              {
                  ItemId = i.ItemId,
                  ItemName = i.Item.ItemName,
                  ItemPrice = i.Item.ItemPrice,
                  ItemPic = i.Item.ItemPic,  
                  Quantity = i.Quantity,
              }).ToList();





              var orderDto = new CreateOrderDTO
              {
                  CartId = cart.CartId,
                  CustomerId = cart.CustomerId,
                  MerchantId = 
                  cart.CartItems?.Select(ci => new GetCartItemsResponseDTO(
                      ci.CartItemId,
                      ci.ItemId,
                      ci.Item.ItemName ?? "Unknown Item",
                      ci.Item.ItemPrice,
                      ci.Item.ItemPic ?? "",
                      ci.Quantity
                  )).ToList() ?? new List<GetCartItemsResponseDTO>()
              };

              return Ok(orderDto);
          }
  */

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
                    ci.Quantity

                )).ToList() ?? new List<GetOrderItemResponseDTO>()
            );

            return Ok(cartDto);
        }


        /*[HttpGet("getOrderByCart")]

        public async Task<ActionResult<CreateOrderDTO>> GetCartByUser()
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
            if(cartDto== null)
            {
                return NotFound();
            }
            var orderItems = cartDto.CartItems.ToList();
            if (orderItems.Count<=0)
            {
                return NotFound();
            }

            var merchantId = cart.CartItems.Select(item => item.Item.MerchantId).FirstOrDefault();
            if(cartDto == null ) {
                return NotFound();
            }
            var merchant = await _context.Merchants.FindAsync(merchantId);
            if(merchant == null ) {
                return NotFound();
            }
            var merchantUser = await _context.Users.FindAsync(merchantId);
           
            if ( merchantUser == null) {
                return NotFound();
            }
            var Address = await _context.Addresses.FindAsync(merchantUser.UserId);
            if (Address == null )
            {
                return NotFound();
            }


            var order = new CreateOrderDTO
            {
                CartId = cartDto.CartId,
                TotalAmount = 0,
                Merchant = new MerchantProfileDTO
                {
                    BusinessName = merchant.BusinessName ?? "",
                    MerchantPic = merchant.MerchantPic,
                    PreparingTime = merchant.PreparingTime,
                    MerchantDescription = merchant.MerchantDescription,
                    UserId = merchant.UserId,
                    FirstName = merchantUser.FirstName,
                    LastName = merchantUser.LastName,
                    Unit = Address.Unit,
                    Address = Address.Address,
                    City = Address.City,
                    Province = Address.Province,
                    Postcode = Address.Postcode

                },
                OrderItems = orderItems


            };
           
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }*/
    }
}