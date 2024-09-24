

namespace OrderDeliverySystem.Share.DTOs.CartDTO
{
    public record AddUpdateCartItemsRequestDTO
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }

        
        public AddUpdateCartItemsRequestDTO(int itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }

}
