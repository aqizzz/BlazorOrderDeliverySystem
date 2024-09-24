namespace OrderDeliverySystem.Share.Data.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public required Customer Customer { get; set; }
        public required ICollection<CartItem> CartItems { get; set; }
    }

}
