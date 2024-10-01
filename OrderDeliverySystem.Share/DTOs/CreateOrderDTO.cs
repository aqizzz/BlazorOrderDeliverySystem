using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.DTOs.CartDTO;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO;
using System;
using static OrderDeliverySystem.Share.Data.Constants;

namespace OrderDeliverySystem.Share.DTOs
{
    public class CreateOrderDTO
    {
       
            public int CartId { get; set; }
        public int CustomerId { get; set; }
        public List<int>MerchantIds  { get; set; }
            public decimal TotalAmount { get; set; }
      
            public List<MerchantProfileDTO> Merchants { get; set; }
             public List<GetOrderItemResponseDTO> CartItems { get; set; }



    }
}
