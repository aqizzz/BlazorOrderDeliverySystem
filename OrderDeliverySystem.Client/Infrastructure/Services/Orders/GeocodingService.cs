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
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response JSON: {json}");
            if (response.IsSuccessStatusCode)
			{
				
				var geocodingResult = JsonSerializer.Deserialize<GeocodingResponse>(json, new JsonSerializerOptions
				{
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                Console.WriteLine($"Geocoding Status: {geocodingResult?.Status}");
                Console.WriteLine($"Number of Results: {geocodingResult?.Results?.Count}"); 

                if (geocodingResult?.Status == "OK" && geocodingResult.Results != null && geocodingResult.Results.Count > 0)
				{
					var geometery = geocodingResult.Results.First().Geometry;
					if (geometery != null)
					{
						var location = geometery.Location;

						if (location != null)
						{
							return (location.Lat, location.Lng);
						}
						else
						{
							throw new Exception("Location data is null.");
						}
					}
					else
					{
                        throw new Exception("Geometry data is null.");
                    }
					
				}
				else
				{
                    throw new Exception($"Unable to retrieve coordinates. Status: {geocodingResult?.Status}. Response:{json}");
                }
			}

			throw new Exception($"Error calling API. Status: {response.StatusCode} - {response.ReasonPhrase}");
		}
	}

	public class GeocodingResponse
	{
		public List<GeocodingResult> Results { get; set; }
		public string Status { get; set; }
	}

	public class GeocodingResult
	{
		public Geometry Geometry { get; set; }
		public string FormattedAddress { get; set; }
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
