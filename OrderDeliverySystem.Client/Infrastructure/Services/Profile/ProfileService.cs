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
        private readonly TokenHelper tokenHelper;

        private const string GetCustomerPath = "api/Profile";
        private const string EditCustomerPath = "api/Profile/edit";
        private const string GetWorkerPath = "api/Profile/worker";
        private const string EditWorkerPath = "api/Profile/worker/edit";

        public ProfileService(
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider,
            IHttpClientFactory httpClientFactory,
            TokenHelper tokenHelper)
        {
            this.localStorage = localStorage;
            this.authenticationStateProvider = authenticationStateProvider;
            this.httpClientFactory = httpClientFactory;
            this.tokenHelper = tokenHelper;
        }
        public async Task<UserProfileDTO> GetCustomerProfile()
        {
            //var token = await localStorage.GetItemAsync<string>("authToken");
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            return await httpClient.GetFromJsonAsync<UserProfileDTO>(GetCustomerPath);
        }

        public async Task<UserProfileDTO> GetCustomerProfile(int userId)
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

        public async Task<Result> UpdateCustomerProfile(UserProfileDTO model)
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

        public async Task<WorkerProfileDTO> GetWorkerProfile()
        {
            var token = await localStorage.GetItemAsync<string>("authToken");
            var httpClient = this.httpClientFactory.CreateClient("API");

            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await httpClient.GetFromJsonAsync<WorkerProfileDTO>(GetWorkerPath);
        }

        public async Task<WorkerProfileDTO> GetWorkerProfile(int userId)
        {
            var token = await localStorage.GetItemAsync<string>("authToken");
            var httpClient = this.httpClientFactory.CreateClient("API");

            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var path = GetWorkerPath + "/" + userId;

            return await httpClient.GetFromJsonAsync<WorkerProfileDTO>(path);
        }
    }
}
