using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OrderDeliverySystem.Client.Infrastructure.Extensions;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;


namespace OrderDeliverySystem.Client.Infrastructure.Services.Item
{
    public class ItemService
    {
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly TokenHelper tokenHelper;


        private const string basePath = "api/items"; // or "api/items/{id}" depending on your routing logic

        public ItemService(
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
        public async Task<Result> CreateMenuItem(CreateItemDTO newItemDto)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            return await httpClient
                .PostAsJsonAsync(basePath, newItemDto)
                .ToResult();
        }


        // Fetch specific merchant item by ID
        public async Task<UpdateItemDTO> GetMerchantItemById(int id)
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var response = await httpClient.GetFromJsonAsync<UpdateItemDTO>($"{basePath}/{id}");
            return response;
        }

        // Update Merchant Items
        public async Task<HttpResponseMessage> UpdateMerchantItem(int id, UpdateItemDTO updateDto)
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var response = await httpClient.PutAsJsonAsync($"{basePath}/{id}", updateDto);
            return response;
        }

        public async Task<List<ViewItemDTO>> GetItems()
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var path = basePath + "/merchant";
            return await httpClient.GetFromJsonAsync<List<ViewItemDTO>>(path);
        }

        public async Task<Result> RemoveItem(int itemId)
        {
            var httpClient = httpClientFactory.CreateClient("API");
            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);
            var path = basePath + "/" + itemId;
            return await httpClient.DeleteAsync(path).ToResult();
        }

    }
}