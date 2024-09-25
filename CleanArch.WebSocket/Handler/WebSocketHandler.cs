using System.Net.WebSockets;
using System.Text;

public static class WebSocketHandler
{
    public static async Task HandleWebSocket(WebSocket webSocket, string clientId)
    {
        WebSocketManager.AddClient(clientId, webSocket);

        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = null;
        try
        {
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received from {clientId}: {message}");

                    // Xử lý dữ liệu từ client
                    await WebSocketManager.SendMessageToAllClients($"Message from {clientId}: {message}");
                }
            } while (!result.CloseStatus.HasValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception for client {clientId}: {ex.Message}");
        }
        finally
        {
            WebSocketManager.RemoveClient(clientId);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            Console.WriteLine($"Client {clientId} disconnected.");
        }
    }
}