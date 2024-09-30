using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDeliverySystem.Share.DTOs.PlacedOrderDTO
{
    
        public record GetOrderItemResponseDTO
        {
            public int CartItemId { get; set; }
            public int MerchantId { get; set; }
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public decimal ItemPrice { get; set; }
            public string ItemPic { get; set; }
            public int Quantity { get; set; }
       


            public GetOrderItemResponseDTO(int cartItemId,   int merchantId ,int itemId, string itemName, decimal itemPrice, string itemPic, int quantity)
        {
                CartItemId = cartItemId;
                 MerchantId = merchantId;
                ItemId = itemId;
                ItemName = itemName;
                ItemPrice = itemPrice;
                ItemPic = itemPic;
                Quantity = quantity;
          
            }
        }
    
}
