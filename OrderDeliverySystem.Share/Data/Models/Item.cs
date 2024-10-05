using System.ComponentModel.DataAnnotations.Schema;

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
        public int IsAvailable { get; set; } 

        [NotMapped]
        public bool ItemIsAvailable
        {
            get => IsAvailable == 1; 
            set => IsAvailable = value ? 1 : 0; 
        }
        public required Merchant Merchant { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }

}
