

namespace OrderDeliverySystem.Share.DTOs.CartDTO
{
    public record GetCartReponseDTO
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public List<GetCartItemsResponseDTO> CartItems { get; set; }

        
        public GetCartReponseDTO(int cartId, int customerId, List<GetCartItemsResponseDTO> cartItems)
        {
            CartId = cartId;
            CustomerId = customerId;
            CartItems = cartItems;
        }
    }
}
