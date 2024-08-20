using CleanArch.Core.Entities.ResponseModel;
using System.Net;
using System.Text.Json;

namespace CleanArch.Api.Filter
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Chuyển request tới middleware tiếp theo trong pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Xử lý exception và trả về phản hồi cho client
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Thiết lập mã trạng thái HTTP cho lỗi nội bộ server
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ServiceResult() {
                Status = false,
                StatusMessage = exception.Message,
                Data = exception.StackTrace
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }

    // Extension method để thêm middleware vào pipeline
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
