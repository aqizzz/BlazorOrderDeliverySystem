using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Share.DTOs.CartDTO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly TokenHelper tokenHelper;

        private const string GetCartPath = "api/cart/getCart";
        private const string AddCartPath = "api/cart/addCart";
        private const string UpdateCartPath = "api/cart/updateCart";
        private const string RemoveItemPath = "api/cart/deleteItem";
        private const string ClearCartPath = "api/cart/clearCart";

        public CartService(
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

        // Get Cart Items
        public async Task<GetCartReponseDTO> GetCartItems()
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var response = await httpClient.GetFromJsonAsync<GetCartReponseDTO>(GetCartPath);
            return response ?? new GetCartReponseDTO(0, 0, new List<GetCartItemsResponseDTO>());
        }

        // Update Cart Items
        public async Task<HttpResponseMessage> UpdateCartItems(AddUpdateCartItemsRequestDTO updateDto)
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            return await httpClient.PutAsJsonAsync(UpdateCartPath, updateDto);
        }

        // Remove Cart Items
        public async Task<HttpResponseMessage> RemoveCartItems(int itemId)
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            return await httpClient.DeleteAsync($"{RemoveItemPath}/{itemId}");
        }

        // Add to Cart Items
        public async Task<HttpResponseMessage> AddToCartItems(List<AddUpdateCartItemsRequestDTO> cartItems)
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            return await httpClient.PostAsJsonAsync(AddCartPath, cartItems);
        }

        // Clear Cart Items
        public async Task<HttpResponseMessage> ClearCartItems()
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            return await httpClient.DeleteAsync(ClearCartPath);
        }
    }
}
