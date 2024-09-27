using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Client.Infrastructure.Extensions;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IHttpClientFactory httpClientFactory;

        private const string GetCustomerPath = "api/Profile";
        private const string EditCustomerPath = "api/Profile/edit";
        //private const string ChangePasswordPath = "api/Auth/me/change-password";

        public ProfileService(
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider,
            IHttpClientFactory httpClientFactory)
        {
            this.localStorage = localStorage;
            this.authenticationStateProvider = authenticationStateProvider;
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<UserProfileDTO> GetCustomer()
        {
            var token = await localStorage.GetItemAsync<string>("authToken");
            var httpClient = this.httpClientFactory.CreateClient("API");

            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await httpClient.GetFromJsonAsync<UserProfileDTO>(GetCustomerPath);
        }

        public async Task<UserProfileDTO> GetCustomer(int userId)
        {
            var token = await localStorage.GetItemAsync<string>("authToken");
            var httpClient = this.httpClientFactory.CreateClient("API");

            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var path = GetCustomerPath + "/" + userId;

            return await httpClient.GetFromJsonAsync<UserProfileDTO>(path);
        }

        public async Task<Result> UpdateCustomer(UserProfileDTO model)
        {
            var token = await localStorage.GetItemAsync<string>("authToken");
            var httpClient = this.httpClientFactory.CreateClient("API");

            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await httpClient
                .PutAsJsonAsync(EditCustomerPath, model)
                .ToResult();
        }
    }
}
