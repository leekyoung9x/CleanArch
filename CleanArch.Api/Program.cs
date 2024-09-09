using CleanArch.Api.DependencyInjection;
using CleanArch.Api.Filter;
using CleanArch.Core.Entities;
using CleanArch.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Nest;
using NLog;
using NLog.Web;
using CleanArch.Core.Entities.Mapper;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Cấu hình NLog
builder.Logging.ClearProviders();
builder.Host.UseNLog(); // Sử dụng NLog để ghi log

// Bind Elasticsearch configuration
var elasticsearchOptions = new ElasticsearchOptions();
builder.Configuration.GetSection("Elasticsearch").Bind(elasticsearchOptions);

// Cấu hình Elasticsearch
builder.Services.AddSingleton<IElasticClient>(provider =>
{
    var settings = new ConnectionSettings(new Uri(elasticsearchOptions.Uri))
        .BasicAuthentication(elasticsearchOptions.Username, elasticsearchOptions.Password)
        .DefaultIndex(elasticsearchOptions.DefaultIndex)
        .DisableDirectStreaming();

    return new ElasticClient(settings);
});

//Injecting services.
builder.Services.RegisterServices();
builder.Services.RegisterServicesAPI();
builder.Services.RegisterMapper();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add services to the container
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Description = "api key.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "basic"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Thêm dịch vụ CORS vào DI container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddLogging();

//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.ListenAnyIP(5000, listenOptions =>
//    {
//        // Tìm đường dẫn tuyệt đối đến chứng chỉ .pfx trong thư mục wwwroot/ssl
//        var pfxPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ssl", "nrohat.pfx");
//        var pfxPassword = "Sang321#@!"; // Thay thế bằng mật khẩu của chứng chỉ .pfx

//        // Cấu hình HTTPS
//        listenOptions.UseHttps(pfxPath, pfxPassword);
//    }); // Lắng nghe trên tất cả IP tại cổng 5000
//});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // Lắng nghe trên tất cả IP tại cổng 5000
});

var app = builder.Build();

app.UseExceptionHandlingMiddleware(); // Thêm middleware vào pipeline

// Sử dụng middleware CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseSwagger();
//app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();