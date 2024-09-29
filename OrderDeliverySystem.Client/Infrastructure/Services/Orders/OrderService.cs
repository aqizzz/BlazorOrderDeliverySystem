﻿using System.Net.Http;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
    public class OrderService
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
        public async Task<List<OrderDTO>> GetOrdersByRoleAsync(string role, int id, bool recent)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}{role}/{id}?recent={recent.ToString().ToLower()}";

			var orders = await httpClient.GetFromJsonAsync<List<OrderDTO>>(uri);
			return orders ?? new List<OrderDTO>();
		}

        public async Task<List<OrderDTO>> GetOrdersTableByRoleAsync(string role, int id, bool recent )
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}/table/{role}/{id}?recent={recent.ToString().ToLower()}";
            var orders = await httpClient.GetFromJsonAsync<List<OrderDTO>>(uri);
            return orders ?? new List<OrderDTO>();
        }

        public async Task<Result> CreateOrderAsync(CreateOrderDTO order)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var uri = $"{Base}/create";

            // Sending the request with payload
            var response = await httpClient.PostAsJsonAsync(uri, order);


            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>();
                return errors != null
                    ? Result.Failure(errors) // Return the errors if present
                    : Result.Failure(new[] { "An unknown error occurred." });
            }

            return Result.Success;
        }

        public async Task<HttpResponseMessage> UpdateOrderAsync(OrderDTO order)
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
            return response;
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var uri = $"{Base}order/{id}";
            var httpClient = this.httpClientFactory.CreateClient("API");
            var order = await httpClient.GetFromJsonAsync<OrderDTO>(uri);
            return order ?? new OrderDTO();
        }
    }
}

