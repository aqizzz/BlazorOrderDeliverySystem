using System.Net;

namespace OrderDeliverySystem.Share.Data.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int MerchantId { get; set; }
        public int? WorkerId { get; set; }
        public int? PickupAddressId { get; set; }
        public int? DropoffAddressId { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required Customer Customer { get; set; }
        public required Merchant Merchant { get; set; }
        public DeliveryWorker? DeliveryWorker { get; set; }
        public AddressModel? PickupAddress { get; set; }
        public AddressModel? DropoffAddress { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }

}
