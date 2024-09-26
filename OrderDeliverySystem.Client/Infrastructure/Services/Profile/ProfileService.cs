using Azure;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OrderDeliverySystem.Client.Infrastructure.Extensions;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.DTOs;
using System.Net.Http.Json;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IHttpClientFactory httpClientFactory;

        private const string GetCustomerPath = "api/Profile/";
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
        public async Task<Result<UserProfileDTO>> GetCustomer(string id)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            return await httpClient.GetAsync(GetCustomerPath + id).ToResult<UserProfileDTO>();
        }

        public async Task<Result> UpdateCustomer(UserProfileDTO model)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            return await httpClient
                .PostAsJsonAsync(EditCustomerPath, model)
                .ToResult();
        }
    }
}
