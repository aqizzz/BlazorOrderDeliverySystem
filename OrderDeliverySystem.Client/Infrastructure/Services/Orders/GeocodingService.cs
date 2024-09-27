using System.Text.Json;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
	public class GeocodingService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly string _googleApiKey = "AIzaSyAFeu-tdOM4SY-z8pfwZ8m35AOT7azMA2E";

		public GeocodingService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<(double lat, double lng)> GetCoordinatesFromAddressAsync(string address)
		{
			var client = _httpClientFactory.CreateClient();
			var requestUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_googleApiKey}";

			var response = await client.GetAsync(requestUrl);
			if (response.IsSuccessStatusCode)
			{
				var json = await response.Content.ReadAsStringAsync();
				var geocodingResult = JsonSerializer.Deserialize<GeocodingResponse>(json);

				if (geocodingResult?.Results != null && geocodingResult.Results.Any())
				{
					var location = geocodingResult.Results.First().Geometry.Location;
					return (location.Lat, location.Lng);
				}
			}

			throw new Exception("Unable to retrieve coordinates.");
		}
	}

	public class GeocodingResponse
	{
		public List<GeocodingResult> Results { get; set; }
	}

	public class GeocodingResult
	{
		public Geometry Geometry { get; set; }
	}

	public class Geometry
	{
		public Location Location { get; set; }
	}

	public class Location
	{
		public double Lat { get; set; }
		public double Lng { get; set; }
	}

}
