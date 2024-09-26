using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDeliverySystem.Share.DTOs
{
    internal class AddressDTO
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string? Type { get; set; }
        public string? Unit { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Postcode { get; set; }
    }
}
