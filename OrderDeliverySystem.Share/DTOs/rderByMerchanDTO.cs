using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDeliverySystem.Share.DTOs
{
    public class OrderByMerchanDTO
    {
        public MerchantProfileDTO? Merchant { get; set; }

        public List<GetOrderItemResponseDTO> CartItems { get; set; } = [];
    }
}
