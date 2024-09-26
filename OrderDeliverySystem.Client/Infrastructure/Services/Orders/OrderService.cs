using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using Azure.Messaging;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;


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

        public async Task<Result> CreateOrderAsync(OrderDTO order)
        {
            var uri = $"{Base}/create";
            Console.WriteLine($"Base Address: {_httpClient.BaseAddress}");
            Console.WriteLine($"making request to {uri}");

            try
            {
                // Sending the request with payload
                var response = await _httpClient.PostAsJsonAsync(uri, order);


                if (!response.IsSuccessStatusCode)
                {
                    var errors = await response.Content.ReadFromJsonAsync<string[]>();
                    return errors != null
                        ? Result.Failure(errors) // Return the errors if present
                        : Result.Failure(new[] { "An unknown error occurred." });
                }

                return Result.Success;
            }
            catch (HttpRequestException ex)
            {
                // Handle network-related exceptions
                Console.WriteLine($"Network error: {ex.Message}");
                return Result.Failure(new[] { ex.Message });
            }
            catch (TaskCanceledException ex)
            {
                // Handle request timeout or cancellations
                Console.WriteLine($"Request timeout: {ex.Message}");
                return Result.Failure(new[] { ex.Message });
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return Result.Failure(new[] { ex.Message });
            }
        }
    }
}

