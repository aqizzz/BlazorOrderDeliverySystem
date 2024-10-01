namespace OrderDeliverySystem.Share.Data.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public required Order Order { get; set; }
        public required Customer Customer { get; set; }

    }

}
