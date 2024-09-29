using OrderDeliverySystem.Share.DTOs.CartDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDeliverySystem.Share.DTOs.PlacedOrderDTO
{


    namespace OrderDeliverySystem.Share.DTOs.CartDTO
    {
        public record GetOrderResponseDTO
        {
            public int CartId { get; set; }
            public int CustomerId { get; set; }
            public List<GetOrderItemResponseDTO> CartItems { get; set; }


            public GetOrderResponseDTO(int cartId, int customerId, List<GetOrderItemResponseDTO> cartItems)
            {
                CartId = cartId;
                CustomerId = customerId;
                CartItems = cartItems;
            }
        }
    }
}
