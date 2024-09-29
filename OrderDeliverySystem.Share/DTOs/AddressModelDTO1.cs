﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDeliverySystem.Share.DTOs
{
    public class AddressModelDTO1
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string? Type { get; set; }
        public string? Unit { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Postcode { get; set; }
        public string FullAddress => $"{Unit} {Address}, {City}, {Province}";
        public  UserDTO1 User { get; set; }
    }
}
