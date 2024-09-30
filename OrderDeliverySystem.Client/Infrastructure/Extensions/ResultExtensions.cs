using OrderDeliverySystem.Share.Data;
using System.Net.Http.Json;
using static OrderDeliverySystem.Client.Infrastructure.Services.Authentication.AuthService;
using System.Text.Json;

namespace OrderDeliverySystem.Client.Infrastructure.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<Result> ToResult(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;

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
    }

}
