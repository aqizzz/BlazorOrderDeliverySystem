using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Client.Infrastructure.Extensions;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IHttpClientFactory httpClientFactory;

        private const string LoginPath = "/api/Auth/login";
        private const string RegisterPath = "/api/Auth/register";
        private const string ChangePasswordPath = "/api/Auth/me/change-password";
        private const string WorkerRegisterPath = "/api/Auth/register/worker";
        private const string MerchantRegisterPath = "/api/Auth/register/merchant";
        public class ErrorResponse
        {
            public string Error { get; set; }
        }

        public AuthService(
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider,
            IHttpClientFactory httpClientFactory)
        {
            this.localStorage = localStorage;
            this.authenticationStateProvider = authenticationStateProvider;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<Result> Register(CustomerRegisterDTO model)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            return await httpClient
                .PostAsJsonAsync(RegisterPath, model)
                .ToResult();
        }

        public async Task<Result> Login(LoginRequestDTO model)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            var response = await httpClient.PostAsJsonAsync(LoginPath, model);

            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>();

                return Result.Failure(errors);
            }

            var responseAsString = await response.Content.ReadAsStringAsync();

            var responseObject = JsonSerializer.Deserialize<LoginResponseDTO>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var token = responseObject.Token;

            await this.localStorage.SetItemAsync("authToken", token);

            ((ApiAuthenticationStateProvider)this.authenticationStateProvider).MarkUserAsAuthenticated(model.Email);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return Result.Success;
        }

        public async Task<Result> ChangePassword(ChangePasswordRequestDto model)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            return await httpClient
                .PostAsJsonAsync(ChangePasswordPath, model)
                .ToResult();
        }

        public async Task Logout()
        {
            await this.localStorage.RemoveItemAsync("authToken");

            ((ApiAuthenticationStateProvider)this.authenticationStateProvider).MarkUserAsLoggedOut();

            var httpClient = this.httpClientFactory.CreateClient("API");
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
