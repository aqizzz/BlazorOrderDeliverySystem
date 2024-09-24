

namespace OrderDeliverySystem.Share.DTOs.CartDTO
{
    public record GetCartItemsResponseDTO
    {
        public int CartItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public string ItemPic { get; set; }
        public int Quantity { get; set; }

        
        public GetCartItemsResponseDTO(int cartItemId, int itemId, string itemName, decimal itemPrice, string itemPic, int quantity)
        {
            CartItemId = cartItemId;
            ItemId = itemId;
            ItemName = itemName;
            ItemPrice = itemPrice;
            ItemPic = itemPic;
            Quantity = quantity;
        }
    }

}
