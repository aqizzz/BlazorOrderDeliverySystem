using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs.ReviewDTO;


namespace OrderDeliverySystem.Client.Infrastructure.Services.Review
{
    public interface IReviewService
    {
        Task<Result> AddReview(CreateReviewRequestDTO reviewDto);
        Task<Result> DeleteReview(int reviewId);
        Task<List<GetReviewResponseDTO>> CustomerGetReviews(int merchantId);
        Task<List<GetReviewResponseDTO>> MerchantGetReviews();
        Task<List<GetReviewResponseDTO>> AdminGetReviews();
        Task<Result> UpdateReply(int reviewId, UpdateReplyRequestDTO replyDto);
        Task<GetReviewResponseDTO> GetReviewByOrderId(int orderId);


    }
}
