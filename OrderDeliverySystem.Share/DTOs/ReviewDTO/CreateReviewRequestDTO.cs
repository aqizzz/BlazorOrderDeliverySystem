

namespace OrderDeliverySystem.Share.DTOs.ReviewDTO
{
    public class CreateReviewRequestDTO
    {
        public int OrderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        
    }


}
