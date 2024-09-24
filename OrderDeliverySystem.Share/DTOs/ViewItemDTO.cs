
using OrderDeliverySystem.Share.Data.Models;

namespace OrderDeliverySystem.Share.DTOs
{
    public class ViewItemDTO
    {
        public int MerchantId { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal ItemPrice { get; set; }
        public string? ItemPic { get; set; }
        public bool ItemIsAvailable { get; set; }
    }
}

