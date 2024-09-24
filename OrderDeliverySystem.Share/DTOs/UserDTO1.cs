using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderDeliverySystem.Share.Data.Models;

namespace OrderDeliverySystem.Share.DTOs
{
    public class UserDTO1
    {
        public int UserId { get; set; }
        public string? PasswordHash { get; set; }
        public required string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public required string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public Customer? Customer { get; set; }
        public DeliveryWorker? DeliveryWorker { get; set; }
        public Merchant? Merchant { get; set; }
        public ICollection<AddressModelDTO1>? Addresses { get; set; }
    }
}
