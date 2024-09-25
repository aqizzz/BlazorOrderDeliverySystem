using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderDeliverySystem.Share.Data.Models;

namespace OrderDeliverySystem.Share.DTOs
{
    public class CustomerDTO1
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public UserDTO1 User { get; set; }

        public ICollection<Cart>? Carts { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
