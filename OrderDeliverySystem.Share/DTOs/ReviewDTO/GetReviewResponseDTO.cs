

namespace OrderDeliverySystem.Share.DTOs.ReviewDTO
{
    public class GetReviewResponseDTO
    {
        public int ReviewId { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public string? Reply { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReplyCreatedAt { get; set; }

        
    }
}
