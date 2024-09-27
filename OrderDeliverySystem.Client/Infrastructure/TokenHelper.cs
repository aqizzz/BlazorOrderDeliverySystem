using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace OrderDeliverySystem.Client.Infrastructure
{
    public class TokenHelper
    {
        private readonly ILocalStorageService _localStorageService;

        public TokenHelper(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task ConfigureHttpClientAuthorization(HttpClient httpClient)
        {
            var token = await _localStorageService.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
