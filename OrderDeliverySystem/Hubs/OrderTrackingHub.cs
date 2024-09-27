using Microsoft.AspNetCore.SignalR;

namespace OrderDeliverySystem.Hubs
{
	public class OrderTrackingHub : Hub
	{
		public async Task UpdateLocation(string orderId, double latitude, double longitude)
		{
			await Clients.Group(orderId).SendAsync("RecieveLocationUpdate", latitude, longitude);
		}

		public async Task TrackOrder(string orderId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
		}
		public async Task StopTrackingOrder(string orderId)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
		}
	}
}
//Changes to WebSocket