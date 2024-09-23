namespace OrderDeliverySystem.Share.Data.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; } // Price at the time of order
        public int OrderId { get; set; } // Foreign key to Order
        public decimal Discount { get; set; } // Discount applied to the order
        public decimal Tax { get; set; } // Tax applied
        public required Order Order { get; set; }
        public required Item Item { get; set; }
    }
}
