using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    // Ví dụ cấu hình Kestrel
    options.ListenAnyIP(6000); // Lắng nghe trên port 5000
    // Bạn có thể thêm các cấu hình khác cho Kestrel tại đây
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var configuration = builder.Configuration;

// Add services to the container
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ClockSkew = TimeSpan.Zero,
//             ValidIssuer = configuration["Jwt:Issuer"],
//             ValidAudience = configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
//         };
//     });

// builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();

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
    
    if (string.IsNullOrEmpty(token) || token == "null")
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

app.Run();
