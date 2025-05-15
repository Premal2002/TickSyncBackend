using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace TickSyncAPI.HelperClasses
{
    public class WebSocketManager
    {
        private readonly ConcurrentDictionary<int, List<WebSocket>> _connections = new();

        public async Task HandleConnectionAsync(int showId, WebSocket socket)
        {
            if (!_connections.ContainsKey(showId))
                _connections[showId] = new List<WebSocket>();

            _connections[showId].Add(socket);

            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;
            }

            _connections[showId].Remove(socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
        }

        public async Task BroadcastAsync(int showId, object message)
        {
            if (!_connections.TryGetValue(showId, out var sockets)) return;

            var msg = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(msg);

            var toRemove = new List<WebSocket>();

            foreach (var socket in sockets)
            {
                if (socket is null) continue;
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    toRemove.Add(socket);
                }
            }

            // Clean up dead sockets
            foreach (var socket in toRemove)
            {
                sockets.Remove(socket);
            }
        }
    }


}
