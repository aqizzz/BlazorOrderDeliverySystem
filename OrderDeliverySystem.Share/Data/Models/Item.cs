namespace OrderDeliverySystem.Share.Data.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public int MerchantId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal ItemPrice { get; set; }
        public string? ItemPic { get; set; }
        public bool ItemIsAvailable { get; set; }
        public required Merchant Merchant { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }

}
