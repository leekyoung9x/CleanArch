using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(MyAllowSpecificOrigins,
//                          policy =>
//                          {
//                              policy.AllowAnyOrigin()
//                                    .AllowAnyHeader()
//                                    .AllowAnyMethod();
//                          });
//});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var configuration = builder.Configuration;

// Cấu hình Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(6000); // Lắng nghe trên tất cả IP tại cổng 5000
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Cấu hình WebSocket
app.UseWebSockets();

// Khởi chạy ConsoleJob trong background để đọc dữ liệu từ console
var consoleJob = new ConsoleJob();
Task.Run(() => consoleJob.StartAsync());

app.MapGet("/ws", async context =>
{
    // Kiểm tra header để xác nhận yêu cầu là WebSocket
    if (context.Request.Headers["Upgrade"].ToString().ToLower() != "websocket" ||
        context.Request.Headers["Connection"].ToString().ToLower() != "upgrade")
    {
        context.Response.StatusCode = 400; // Bad Request
        return;
    }

    // Lấy token từ query parameters
    var token = context.Request.Query["token"].ToString();

    if (string.IsNullOrEmpty(token) || token == "null" || token == null)
    {
        context.Response.StatusCode = 401; // Unauthorized
        return;
    }

    // Xác thực JWT và lấy client ID
    var handler = new JwtSecurityTokenHandler();
    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
    var clientId = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;

    if (string.IsNullOrEmpty(clientId))
    {
        context.Response.StatusCode = 401; // Unauthorized
        return;
    }

    // Thiết lập kết nối WebSocket
    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    await WebSocketHandler.HandleWebSocket(webSocket, clientId);
});

app.MapGet("/send", async context =>
{
    // Kiểm tra header để xác nhận yêu cầu là WebSocket
    if (context.Request.Headers["Upgrade"].ToString().ToLower() != "websocket" ||
        context.Request.Headers["Connection"].ToString().ToLower() != "upgrade")
    {
        context.Response.StatusCode = 400; // Bad Request
        return;
    }

    var data = context.Request.Query["data"].ToString();

    // Decode the data
    var decodedData = Uri.UnescapeDataString(data);

    string id = decodedData.Split('|')[0];
    string message = decodedData.Split('|')[1];

    await WebSocketManager.SendMessageToClient(id, message);
});

// Sử dụng CORS
//app.UseCors();

app.Run();
