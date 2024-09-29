using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.DTOs.CartDTO;

namespace OrderDeliverySystem.Share.DTOs
{
    public class CreateOrderDTO
    {
        public MerchantProfileDTO Merchant { get; set; }
        public List<CreateItemDTO> OrderItems { get; set; }
    }
}
