
using System.Text.Json;


namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
	public class DirectionsService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly string _googleApiKey = "AIzaSyAFeu-tdOM4SY-z8pfwZ8m35AOT7azMA2E";

		public DirectionsService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<DirectionsResult> GetDirectionsAsync(double originLat, double originLng, double destLat, double destLng)
		{
			var client = _httpClientFactory.CreateClient();
			var requestUrl = $"https://maps.googleapis.com/maps/api/directions/json?origin={originLat},{originLng}&destination={destLat},{destLng}&key={_googleApiKey}";

			var response = await client.GetAsync(requestUrl);
			if (response.IsSuccessStatusCode)
			{
				var json = await response.Content.ReadAsStringAsync();
				var directionsResult = JsonSerializer.Deserialize<DirectionsResult>(json);
				return directionsResult;
			}

			throw new Exception("Unable to retrieve directions.");
		}
	}

	public class DirectionsResult
	{
		public List<Route> Routes { get; set; }
	}

	public class Route
	{
		public List<Leg> Legs { get; set; }
		public OverviewPolyline OverviewPolyline { get; set; }
	}

	public class Leg
	{
		public string StartAddress { get; set; }
		public string EndAddress { get; set; }
		public List<Step> Steps { get; set; }
	}

	public class Step
	{
		public string HtmlInstructions { get; set; }
	}

	public class OverviewPolyline
	{
		public string Points { get; set; }
	}

}
