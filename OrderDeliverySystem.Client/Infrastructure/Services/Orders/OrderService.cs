using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using OrderDeliverySystem.Share.DTOs;


namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private const string Base = "https://localhost:7027/api/Orders/";

		public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
         public async Task<List<OrderDTO>> GetOrdersByRoleAsync(string role, int id, bool recent)
        {
            var uri = $"{Base}{role}/{id}?recent={recent.ToString().ToLower()}";
            Console.WriteLine($"making request to { uri}");
			var orders = await _httpClient.GetFromJsonAsync<List<OrderDTO>>(uri);
			return orders ?? new List<OrderDTO>();
		}
    }
}
