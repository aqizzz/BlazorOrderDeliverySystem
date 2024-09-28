using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static MudBlazor.Icons;
using static OrderDeliverySystem.Share.Data.Constants;
using OrderDeliverySystem.Share.DTOs.CartDTO;
using OrderDeliverySystem.Client.Shared;

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
            var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var loggedInUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!User.IsInRole("Customer,Merchant,Worker") || loggedInUserRole == null)
            {
                return Forbid();
            }

            var user = await _context.Users
                .Where(u => u.UserId == loggedInUserId )
                .Include(o=> o.Customer)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound($"No worker was found.");
            }

            var cart = await _context.Carts
                .Include(o => o.Customer)
                .Include (o => o.CartItems)
                .ThenInclude(ci=> ci.Item)
                .ThenInclude(cii => cii.Merchant)
                .ThenInclude(ciim => ciim.User)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                return NotFound($"addresses");
            }


            var worker = await _context.DeliveryWorkers
                 .Where(w => w.WorkerAvailability == true && w.LastTaskAssigned != null) 
                 .OrderBy(w => w.LastTaskAssigned) 
                 .FirstOrDefaultAsync();
             if (worker == null)
             {
                 return NotFound($"No worker was found.");
             }

             var customerAddresses = await _context.Addresses
                 .Where(a => a.UserId == loggedInUserId)
                 .ToListAsync(); 

             var isExist = false;
             int dropoffAddressId=0;

             if (customerAddresses == null)
             {
                 return NotFound($"Customer with ID {orderDto.CustomerId} not found.");
             }
             else
             {
                 foreach (var address in customerAddresses)
                 {
                    if(address == null) continue;
                    if (address.Unit!=null && address.Address != null && address.City != null && address.Province != null && address.Postcode != null)
                     {
                         if (address.Unit.Equals(orderDto.Unit) && address.Address.Equals(orderDto.Address) && address.City.Equals(orderDto.City) && address.Province.Equals(orderDto.Province) && address.Postcode.Equals(orderDto.PostCode))
                         {
                             isExist = true;
                             dropoffAddressId = address.AddressId;
                             break;
                         }
                     }

                 }
             }
            
             if (!isExist) {
                 AddressModel newAddress = new AddressModel()
                 {
                     UserId = loggedInUserId,
                     User = user,
                     Type = "",
                     Unit = orderDto.Unit,
                     Address = orderDto.Address,
                     City = orderDto.City,
                     Province = orderDto.Province,
                     Postcode = orderDto.PostCode
                 };

                 _context.Addresses.Add(newAddress);
                 await _context.SaveChangesAsync();
                 dropoffAddressId = newAddress.AddressId;
             }

             if (dropoffAddressId <= 0)
             {
                 return NotFound($"Address with ID {dropoffAddressId} not found.");
             }

             var meechantAddress = await _context.Addresses.FindAsync(orderDto.MerchantId);


             if (meechantAddress == null)
             {
                 return NotFound($"Customer with ID {orderDto.CustomerId} not found.");
             }

             var order = new Order
             {
                 CustomerId = orderDto.CustomerId,
                 MerchantId = orderDto.MerchantId,
                 WorkerId = worker.WorkerId,
                 PickupAddressId = meechantAddress.AddressId,
                 DropoffAddressId = dropoffAddressId,
                 TotalAmount = orderDto.TotalAmount,
                 Status = "Pending",
                 CreatedAt = DateTime.Now,
                 UpdatedAt = DateTime.Now,
                 Merchant = user.,
                 Customer = user.Customer,


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

         public async Task<IActionResult> UpdateOrder( int id ,OrderDTO updatedOrder)
         {
             if (id == null)
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

             return Ok(order);
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

             return Ok(order);
         }


         [HttpGet("{role}/{id}")]
         public async Task<IActionResult> GetOrdersByRole(string role, int id,  bool recent = false)
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
         public async Task<IActionResult> GetOrdersTableByRole(string role ,int userId, bool recent, int pageNumber = 1, int pageSize = 10)
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


        [HttpGet]
        [Authorize(Roles = "Customer, Merchant,Worder")]
        public async Task<IActionResult> GetUserOrders(bool recent)
         {
            var loggedInUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var loggedInUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!User.IsInRole("Customer,Merchant,Worker") || loggedInUserRole != null)
            {
                return Forbid();
            }


            IQueryable<Order> query = _context.Orders
                 .Include(o => o.Customer)
                 .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                 .Include(o => o.Merchant)
                 .Include(o => o.DeliveryWorker);

             switch (loggedInUserRole?.ToLower())
             {
                 case "customer":
                     query = query.Where(o => o.Customer.UserId == loggedInUserId); break;
                 case "merchant":
                     query = query.Where(o => o.Merchant.UserId == loggedInUserId); break;
                 case "worker":
                    query = query.Where(o => o.DeliveryWorker !=null && o.DeliveryWorker.UserId == loggedInUserId); break;    
             }

            if (recent)
            {
                query = query.Where(o => o.Status != "Delivered");
            }
            else
            {
                query = query.Where(o => o.Status == "Delivered" || o.Status == "Cancelled");
            }

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

            return Ok(orders);

         }


        private async Task<List<OrderDTO>> GetOrders(string query,bool orderType)
        {
           
        }

    }
}
