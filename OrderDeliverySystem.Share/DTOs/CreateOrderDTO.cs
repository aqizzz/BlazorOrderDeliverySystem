using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.DTOs.CartDTO;
using System;
using static OrderDeliverySystem.Share.Data.Constants;

namespace OrderDeliverySystem.Share.DTOs
{
    public class CreateOrderDTO
    {
       
            public int CartId { get; set; }
            public int CustomerId { get; set; }
             public int MerchantId { get; set; }
            public decimal TotalAmount { get; set; }
      
            public MerchantProfileDTO Merchant { get; set; }
             public List<CreateItemDTO> CartItems { get; set; }



    }
}
