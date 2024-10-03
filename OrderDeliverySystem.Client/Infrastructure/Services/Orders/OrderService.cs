using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Client.Infrastructure.Extensions;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.Data.Models;
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
            var uri = $"{Base}/create";
            return await httpClient.PostAsJsonAsync(uri, order).ToResult() ;
        }
    

        public async Task<Result> UpdateOrder(UpdateOrderDTO order)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var uri = $"{Base}/update";
            return await httpClient.PutAsJsonAsync(uri, order).ToResult(); 
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


        public async Task<Result> CancelOrder(UpdateOrderDTO order)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var uri = $"{Base}/cancel";
            return await httpClient.PutAsJsonAsync(uri, order).ToResult();
        }

        public async Task<Result> ApproveOrder(UpdateOrderDTO order)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var uri = $"{Base}/approve";
            return await httpClient.PutAsJsonAsync(uri, order).ToResult();
        }

        public async Task<Result> AssignOrder(UpdateOrderDTO order)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var uri = $"{Base}/assign";
            return await httpClient.PutAsJsonAsync(uri, order).ToResult();
        }

        public async Task<Result> FinishOrder(UpdateOrderDTO order)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var uri = $"{Base}/finish";
            return await httpClient.PutAsJsonAsync(uri, order).ToResult();
        }
    }
}

