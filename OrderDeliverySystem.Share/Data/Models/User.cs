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
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public Customer? Customer { get; set; }
        public DeliveryWorker? DeliveryWorker { get; set; }
        public Merchant? Merchant { get; set; }
        public ICollection<AddressModel>? Addresses { get; set; }
    }
}

