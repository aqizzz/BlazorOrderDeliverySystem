using OrderDeliverySystem.Share.DTOs.ReviewDTO;


namespace OrderDeliverySystem.Client.Infrastructure.Services.Review
{
    public interface IReviewService
    {
        Task<HttpResponseMessage> AddReview(CreateReviewRequestDTO reviewDto);
        Task<HttpResponseMessage> DeleteReview(int reviewId);
        Task<List<GetReviewResponseDTO>> CustomerGetReviews(int merchantId);
        Task<List<GetReviewResponseDTO>> MerchantGetReviews();
        Task<List<GetReviewResponseDTO>> AdminGetReviews();
        Task<HttpResponseMessage> UpdateReply(int reviewId, UpdateReplyRequestDTO replyDto);
        

    }
}
