using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.DTOs.CartDTO;

namespace OrderDeliverySystem.Share.DTOs
{
    public class CreateOrderDTO
    {
        public int CustomerId { get; set; }
        public int MerchantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Unit { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostCode { get; set; }
        public decimal TotalAmount { get; set; }
        public int CartId { get; set; }
        public List<GetCartItemsResponseDTO> OrderItems { get; set; }
    }
}
