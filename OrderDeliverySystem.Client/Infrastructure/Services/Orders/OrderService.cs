using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.DTOs.CartDTO;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO.OrderDeliverySystem.Share.DTOs.CartDTO;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly TokenHelper tokenHelper;

        private const string Base = "api/Orders/";

        public OrderService(
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


        public class ErrorResponse
        {
            public string Error { get; set; }
        }
        public async Task<List<OrderDTO>> GetOrdersByRole(string role, int id, bool recent)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}{role}/{id}?recent={recent.ToString().ToLower()}";

			var orders = await httpClient.GetFromJsonAsync<List<OrderDTO>>(uri);
			return orders ?? new List<OrderDTO>();
		}

        public async Task<List<OrderDTO>> GetOrdersTableByRole(string role, int id, bool recent )
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}table/{role}/{id}?recent={recent.ToString().ToLower()}";
            var orders = await httpClient.GetFromJsonAsync<List<OrderDTO>>(uri);
            return orders ?? new List<OrderDTO>();
        }

        public async Task<Result> CreateOrder(List<GetOrderItemResponseDTO> cartItems)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}create";

            // Sending the request with payload
            var response = await httpClient.PostAsJsonAsync(uri, cartItems);


            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();


                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });


                return Result.Failure(errorResponse?.Error ?? "An unknown error occurred.");
            }

            return Result.Success;
        }

        public async Task<Result> UpdateOrder(OrderDTO order)
        {
            switch (order.Status)
            {
                case "Pending":
                    order.Status = "Approved"; break;
                case "Approved":
                    order.Status = "In Delivery"; break;
                case "In Delivery":
                    order.Status = "Delivered"; break;
            }

            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var uri = $"{Base}update/{order.OrderId}";
            var response = await httpClient.PutAsJsonAsync(uri, order);
            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string>();
                return errors != null
                    ? Result.Failure(errors) // Return the errors if present
                    : Result.Failure( "An unknown error occurred.");
            }
            return Result.Success;
        }

        public async Task<CreateOrderDTO> GetPlacedOrder(int cartId)
        {
            
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var uri = $"{Base}getOrderByCart/{cartId}";

            return await httpClient.GetFromJsonAsync<CreateOrderDTO>(uri);
        }


        public async Task<GetOrderResponseDTO> GetOrderByCart()
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}getCart";
           
            return await httpClient.GetFromJsonAsync<GetOrderResponseDTO>(uri);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            var uri = $"{Base}order/{id}";
            Console.WriteLine($"making request to {uri}");
            var order = await httpClient.GetFromJsonAsync<OrderDTO>(uri);
            return order ?? new OrderDTO();
        }


        public Task<Result> CancelOrder(int orderId)
        {
            throw new NotImplementedException();
        }

    }
}

