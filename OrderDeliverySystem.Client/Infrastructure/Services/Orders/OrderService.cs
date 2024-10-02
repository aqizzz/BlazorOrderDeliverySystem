using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.Arm;
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

        private const string Base = "api/Orders";

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
        public async Task<List<OrderDTO>> GetOrdersByRole(string role, bool recent)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}/get-{role}?recent={recent.ToString().ToLower()}";

			var orders = await httpClient.GetFromJsonAsync<List<OrderDTO>>(uri);
			return orders ?? new List<OrderDTO>();
		}

        public async Task<List<OrderDTO>> GetOrdersTableByRole(string role,  bool recent )
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}/table/{role}?recent={recent.ToString().ToLower()}";
            var orders = await httpClient.GetFromJsonAsync<List<OrderDTO>>(uri);
            return orders ?? new List<OrderDTO>();
        }

        public async Task<Result> CreateOrder(CreateOrderDTO order)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}";
            var response = await httpClient.PostAsJsonAsync(uri, order);
            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string>();
                return errors != null
                    ? Result.Failure(errors) // Return the errors if present
                    : Result.Failure("An unknown error occurred.");
            }
            return Result.Success;
        }
    

        public async Task<Result> UpdateOrder(UpdateOrderDTO order)
        {

          
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var uri = $"{Base}/update";
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



        public async Task<GetOrderResponseDTO> GetOrderByCart()
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}/getCart";
           
            return await httpClient.GetFromJsonAsync<GetOrderResponseDTO>(uri);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}/order/{id}";
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

