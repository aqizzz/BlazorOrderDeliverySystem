using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.DTOs.CartDTO;

namespace OrderDeliverySystem.Share.DTOs
{
    public class CreateOrderDTO
    {
        public Customer Customer { get; set; }
        public List<MerchantProfileDTO> Merchants { get; set; }
        public List<CreateItemDTO> OrderItems { get; set; }
    }
}
