using OrderDeliverySystem.Share.Data;
using System.Net.Http.Json;

namespace OrderDeliverySystem.Client.Infrastructure.Extensions
{
    public static class ResultExtensions
    {
        // For Result type without data
        public static async Task<Result> ToResult(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;

            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>();
                return Result.Failure(errors); // Ensure that this is not treated as a generic call
            }

            return Result.Success; // Return success
        }

        // For Result<TData> type with data
        public static async Task<Result<TData>> ToResult<TData>(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;

            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadFromJsonAsync<string[]>();
                return Result<TData>.Failure(errors); // This is the correct generic call
            }

            var data = await response.Content.ReadFromJsonAsync<TData>(); // Deserialize the response data
            return Result<TData>.SuccessWith(data); // Return the successful Result<TData>
        }
    }

}
