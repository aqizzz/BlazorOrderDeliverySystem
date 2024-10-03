using System.ComponentModel.DataAnnotations.Schema;

namespace OrderDeliverySystem.Share.Data.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? PasswordHash { get; set; }
        public required string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public required string Role { get; set; }
        public int IsActived { get; set; }

        [NotMapped]
        public bool IsActive
        {
            get => IsActived == 1;
            set => IsActived = value ? 1 : 0;
        }
        public DateTime CreatedAt { get; set; }

        public Customer? Customer { get; set; }
        public DeliveryWorker? DeliveryWorker { get; set; }
        public Merchant? Merchant { get; set; }
        public ICollection<AddressModel>? Addresses { get; set; }
    }
}

