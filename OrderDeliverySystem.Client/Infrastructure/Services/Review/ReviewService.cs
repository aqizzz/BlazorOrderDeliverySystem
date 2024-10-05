using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Share.DTOs.ReviewDTO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Client.Infrastructure.Extensions;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Review
{
    public class ReviewService : IReviewService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenHelper _tokenHelper;

        private const string AddReviewPath = "api/review/addReview";
        private const string DeleteReviewPath = "api/review/deleteReview";
        private const string ReviewsByMerchantPath = "api/review/customerReviews";
        private const string MerchantReviewsPath = "api/review/merchantReviews";
        private const string AdminReviewsPath = "api/review/adminReviews";
        private const string UpdateReplyPath = "api/review/updateReply";

    

        public ReviewService(IHttpClientFactory httpClientFactory, TokenHelper tokenHelper)
        {
            _httpClientFactory = httpClientFactory;
            _tokenHelper = tokenHelper;
        }

        // Add Review
        public async Task<Result> AddReview(CreateReviewRequestDTO reviewDto)
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            await _tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            return await httpClient.PostAsJsonAsync(AddReviewPath, reviewDto).ToResult();
        }

        // Delete Review
        public async Task<Result> DeleteReview(int reviewId)
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            await _tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{DeleteReviewPath}/{reviewId}";
            return await httpClient.DeleteAsync(uri).ToResult();
        }

        // Get Reviews by Merchant
        public async Task<List<GetReviewResponseDTO>> CustomerGetReviews(int userId)
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            await _tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var response = await httpClient.GetFromJsonAsync<List<GetReviewResponseDTO>>($"{ReviewsByMerchantPath}/{userId}");
            return response ?? new List<GetReviewResponseDTO>();
        }

        // Get Merchant's Own Reviews
        public async Task<List<GetReviewResponseDTO>> MerchantGetReviews()
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            await _tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var response = await httpClient.GetFromJsonAsync<List<GetReviewResponseDTO>>(MerchantReviewsPath);
            return response ?? new List<GetReviewResponseDTO>();
        }

        // // Get all Reviews
        public async Task<List<GetReviewResponseDTO>> AdminGetReviews()
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            await _tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var response = await httpClient.GetFromJsonAsync<List<GetReviewResponseDTO>>(AdminReviewsPath);
            return response ?? new List<GetReviewResponseDTO>();
        }

        // Update Reply
        public async Task<Result> UpdateReply(int reviewId, UpdateReplyRequestDTO replyDto)
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            await _tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{UpdateReplyPath}/{reviewId}";
            return await httpClient.PutAsJsonAsync(uri, replyDto).ToResult();
        }
    }
}
