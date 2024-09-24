using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderDeliverySystem.Share.Data.Models;

namespace OrderDeliverySystem.Share.DTOs
{
    public class MerchantDTO1
    {
        public int MerchantId { get; set; }
        public int UserId { get; set; }
        public string? BusinessName { get; set; }
        public string? MerchantPic { get; set; }
        public string? MerchantDescription { get; set; }
        public int? PreparingTime { get; set; }
        public  UserDTO1 User { get; set; }

        public ICollection<Item>? Items { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
