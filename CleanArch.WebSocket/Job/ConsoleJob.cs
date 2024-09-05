using System.Threading.Tasks;

public class ConsoleJob
{
    public async Task StartAsync()
    {
        while (true)
        {
            // Đọc input từ console
            var input = await Task.Run(() => Console.ReadLine());

            if (!string.IsNullOrWhiteSpace(input))
            {
                // Gửi tin nhắn tới tất cả các client WebSocket
                await WebSocketManager.SendMessageToAllClients(input);
                Console.WriteLine($"Message sent to all clients: {input}");
            }
        }
    }
}
