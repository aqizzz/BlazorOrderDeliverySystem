using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Share.DTOs.ReviewDTO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

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
        public async Task<HttpResponseMessage> AddReview(CreateReviewRequestDTO reviewDto)
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            await _tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var response = await httpClient.PostAsJsonAsync(AddReviewPath, reviewDto);
            return response;
        }

        // Delete Review
        public async Task<HttpResponseMessage> DeleteReview(int reviewId)
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            await _tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var response = await httpClient.DeleteAsync($"{DeleteReviewPath}/{reviewId}");
            return response;
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
        public async Task<HttpResponseMessage> UpdateReply(int reviewId, UpdateReplyRequestDTO replyDto)
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            await _tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var response = await httpClient.PutAsJsonAsync($"{UpdateReplyPath}/{reviewId}", replyDto);
            return response;
        }
    }
}
