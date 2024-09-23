using OrderDeliverySystem.Share.Data.Models;
namespace OrderDeliverySystem.Share.DTOs
{
    public class AppOrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
