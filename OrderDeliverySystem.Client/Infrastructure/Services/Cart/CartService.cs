using System.Net.Http.Json;
using OrderDeliverySystem.Share.DTOs.CartDTO;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Cart
{
    public class CartService
    {
        private readonly HttpClient _httpClient;

        private const string BaseUrl = "https://localhost:7027/api/cart/";

        public CartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetCartReponseDTO> GetCartItems(string customerId)
        {
            var response = await _httpClient.GetFromJsonAsync<GetCartReponseDTO>($"{BaseUrl}getCart/{customerId}");
            return response ?? new GetCartReponseDTO(0, int.Parse(customerId), new List<GetCartItemsResponseDTO>());
        }

        public async Task<HttpResponseMessage> UpdateCartItems(string customerId, AddUpdateCartItemsRequestDTO updateDto)
        {
            return await _httpClient.PutAsJsonAsync($"{BaseUrl}updateCart/{customerId}", updateDto);
        }

        public async Task<HttpResponseMessage> RemoveCartItems(string customerId, int itemId)
        {
            return await _httpClient.DeleteAsync($"{BaseUrl}deleteItem/{customerId}/{itemId}");
        }

        public async Task<HttpResponseMessage> AddToCartItems(int customerId, List<AddUpdateCartItemsRequestDTO> cartItems)
        {
            return await _httpClient.PostAsJsonAsync($"{BaseUrl}addCart/{customerId}", cartItems);
        }
    }
}
