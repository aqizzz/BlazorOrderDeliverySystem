using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Client.Infrastructure.Extensions;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using System.Net.Http.Json;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Profile
{
    public class AddressService : IAddressService
    {
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly TokenHelper tokenHelper;

        private const string GetAddressListPath = "api/Address/list";
        private const string BasePath = "api/Address";

        public AddressService(
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

        public async Task<List<AddressUpdateDTO>> GetAddressList()
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var response = await httpClient.GetFromJsonAsync<List<AddressUpdateDTO>>(GetAddressListPath);

            return response ?? new List<AddressUpdateDTO>();
        }

        public async Task<AddressUpdateDTO> GetAddress(int addressId)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            var path = BasePath + "/" + addressId;

            var response = await httpClient.GetFromJsonAsync<AddressUpdateDTO>(path);

            return response ?? new AddressUpdateDTO();
        }

        public async Task<Result> UpdateAddress(AddressUpdateDTO addressDto)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            return await httpClient
                .PutAsJsonAsync(BasePath, addressDto)
                .ToResult();
        }

        public async Task<Result> CreateAddress(AddressUpdateDTO addressDto)
        {
            var httpClient = this.httpClientFactory.CreateClient("API");

            await tokenHelper.ConfigureHttpClientAuthorization(httpClient);

            return await httpClient
                .PostAsJsonAsync(BasePath, addressDto)
                .ToResult();
        }


    }
}
