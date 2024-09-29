using Microsoft.AspNetCore.SignalR.Client;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
	public class TrackingService
	{
		private HubConnection _hubConnection;
		private event Action<double, double>? OnLocationUpdated;

		public async Task StartTrackingOrder(string orderId)
		{
			_hubConnection = new HubConnectionBuilder()
				.WithUrl("https://localhost:7027/orderTrackingHub")
				.Build();

			_hubConnection.On<double, double>("ReceiveLocationUpdate", (latitude, longitude) =>
			{
				OnLocationUpdated?.Invoke(latitude, longitude);
			});
			await _hubConnection.StartAsync();
			await _hubConnection.SendAsync("TrackOrder", orderId);
		}

		public async Task StopTrackingOrder(string orderId)
		{
			await _hubConnection.SendAsync("StopTrackingOrder", orderId);
			await _hubConnection.StopAsync();
		}
	}
}
