using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.Data.Models;
using Microsoft.IdentityModel.Tokens;

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
           
            var worker = await _context.DeliveryWorkers
                .Where(w => w.WorkerAvailability == true && w.LastTaskAssigned != null) // Ensure worker is available and has a task history
                .OrderBy(w => w.LastTaskAssigned) // Get the worker with the oldest LastTaskAssigned date
                .FirstOrDefaultAsync();
            if (worker == null)
            {
                return NotFound($"No worker was found.");
            }

            var customerAddresses = await _context.Addresses
                .Where(a => a.UserId == customer.UserId)
                .ToListAsync(); // ToListAsync if it's a collection
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
                    UserId = customer.UserId,
                    User = customer.User,
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
                    PickupAddressId = o.PickupAddressId,
                    DropoffAddressId = o.DropoffAddressId,
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
        [HttpGet("order/{id}")]
        public async Task<IActionResult> GetOrderByID( int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Include(o => o.Merchant)
                .Include(o => o.DeliveryWorker)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if(order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            var orderDTO = new OrderDTO
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                MerchantId = order.MerchantId,
                WorkerId = order.WorkerId,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                PickupAddressId = order.PickupAddressId,
                DropoffAddressId = order.DropoffAddressId,
                Customer = new CustomerDTO1
                {
                    CustomerId = order.Customer.CustomerId,
                    User = new UserDTO1
                    {
                        UserId = order.Customer.User.UserId,
                        FirstName = order.Customer.User.FirstName,
                        LastName = order.Customer.User.LastName,
                        Email = order.Customer.User.Email,
                        Phone = order.Customer.User.Phone,
                        Role = order.Customer.User.Role,
                        Addresses = order.Customer.User.Addresses.Select(a => new AddressModelDTO1
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
                    MerchantId = order.Merchant.MerchantId,
                    BusinessName = order.Merchant.BusinessName,
                    MerchantPic = order.Merchant.MerchantPic,
                    MerchantDescription = order.Merchant.MerchantDescription,
                    PreparingTime = order.Merchant.PreparingTime,
                    User = new UserDTO1
                    {
                        UserId = order.Merchant.User.UserId,
                        FirstName = order.Merchant.User.FirstName,
                        LastName = order.Merchant.User.LastName,
                        Email = order.Merchant.User.Email,
                        Phone = order.Merchant.User.Phone,
                        Role = order.Merchant.User.Role,
                        Addresses = order.Merchant.User.Addresses.Select(a => new AddressModelDTO1
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
                OrderItems = order.OrderItems.Select(oi => new AppOrderItem
                {
                    OrderItemId = oi.OrderItemId,
                    ItemId = oi.ItemId,
                    OrderId = oi.OrderId,
                    Quantity = oi.Quantity
                }).ToList()
            };
            return Ok(orderDTO);

        }
    }
}
