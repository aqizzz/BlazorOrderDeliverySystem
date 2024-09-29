using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace OrderDeliverySystem.Handler
{
    public class WebSocketHandler
    {
        private readonly WebSocket _socket;

        public WebSocketHandler(WebSocket socket)
        {
            _socket = socket;
        }

        public async Task Handle()
        {
            while (_socket.State == WebSocketState.Open)
            {
                var buffer = new byte[1024];
                var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                // Handle incoming messages if needed
                // For example, parse the message and update the worker's location
            }
        }

        public async Task SendLocationAsync(string location)
        {
            if (_socket.State == WebSocketState.Open)
            {
                var message = Encoding.UTF8.GetBytes(location);
                await _socket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        }
    }

