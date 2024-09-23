namespace OrderDeliverySystem.Share.Data.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int ItemId { get; set; }
        public int CartId { get; set; } // Foreign key to Cart
        public int Quantity { get; set; }
        public required Cart Cart { get; set; }
        public required Item Item { get; set; }
    }

}
