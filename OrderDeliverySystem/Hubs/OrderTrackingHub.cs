using Microsoft.AspNetCore.SignalR;

namespace OrderDeliverySystem.Hubs
{
	public class OrderTrackingHub : Hub
	{
		public async Task UpdateLocation(string orderId, double latitude, double longitude)
		{
			await Clients.Group(orderId).SendAsync("ReceiveLocationUpdate", latitude, longitude);
		}

		public async Task TrackOrder(string orderId)
		{
            if (string.IsNullOrEmpty(orderId))
            {
                Console.WriteLine("Invalid orderId received.");
                throw new ArgumentException("Order ID cannot be null or empty.", nameof(orderId));
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
		}
		public async Task StopTrackingOrder(string orderId)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
		}
	}
}
//Changes to WebSocket