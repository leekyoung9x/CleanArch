using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text;

public static class WebSocketManager
{
    private static readonly ConcurrentDictionary<string, WebSocket> _clients = new();

    // Thêm client vào danh sách
    public static void AddClient(string id, WebSocket webSocket)
    {
        _clients.TryAdd(id, webSocket);
        Console.WriteLine("Da them client: " + id);
    }

    // Xóa client khỏi danh sách
    public static void RemoveClient(string id)
    {
        _clients.TryRemove(id, out _);
    }

    // Lấy client từ danh sách
    public static WebSocket GetClient(string id)
    {
        _clients.TryGetValue(id, out var webSocket);
        return webSocket;
    }

    // Gửi tin nhắn đến tất cả client
    public static async Task SendMessageToAllClients(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        foreach (var client in _clients.Values)
        {
            if (client.State == WebSocketState.Open)
            {
                await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}