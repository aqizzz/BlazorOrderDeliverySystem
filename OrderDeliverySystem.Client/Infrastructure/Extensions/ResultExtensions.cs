using OrderDeliverySystem.Share.Data;
using System.Net.Http.Json;

namespace OrderDeliverySystem.Client.Infrastructure.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<Result> ToResult(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;

            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>();

                return Result.Failure(errors);
            }

            return Result.Success;
        }
    }

}
