namespace OrderDeliverySystem.Share.Data.Models
{
    public class Merchant
    {
        public int MerchantId { get; set; }
        public int UserId { get; set; }
        public string? BusinessName { get; set; }
        public string? MerchantPic { get; set; }
        public string? MerchantDescription { get; set; }
        public int? PreparingTime { get; set; }
        public required User User { get; set; }

        public ICollection<Item>? Items { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }

}
