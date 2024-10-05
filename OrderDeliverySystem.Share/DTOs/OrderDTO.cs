using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.DTOs.ReviewDTO;

namespace OrderDeliverySystem.Share.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int MerchantId { get; set; }
        public int? WorkerId { get; set; }
        public int? PickupAddressId { get; set; }
        public int? DropoffAddressId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public  CustomerDTO1 Customer { get; set; }
        public  MerchantDTO1 Merchant { get; set; }
        public WorkerDTO1? DeliveryWorker { get; set; }
        public ICollection<AppOrderItem> OrderItems { get; set; }
        public GetReviewResponseDTO? Reviews { get; set; }
    }
}
