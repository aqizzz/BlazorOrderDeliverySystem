using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace OrderDeliverySystem.Hubs
{
    public class OrderTrackingHub : Hub
    {
        public async Task UpdateWorkerLocation(int orderId, double latitude, double longitude)
        {
            if (string.IsNullOrEmpty(orderId.ToString()))
            {
                throw new ArgumentException("Order ID cannot be null or empty");
            }
            await Clients.Group(orderId.ToString()).SendAsync("ReceiveLocationUpdate", latitude, longitude);
        }

        public async Task TrackOrder(int orderId)
        {
            if (orderId == null)
            {
                Console.WriteLine("Invalid orderId received.");
                throw new ArgumentException("Order ID cannot be null or empty.", nameof(orderId));
            }
            Console.WriteLine($"Tracking order with ID: {orderId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId.ToString());
        }
        public async Task StopTrackingOrder(int orderId)
        {
            Console.WriteLine($"Stopped tracking order with ID: {orderId}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId.ToString());
        }
    }
}