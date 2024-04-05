using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env )
        {
            _logger = logger;
            _env = env;
            _next = next;  

        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                    ? new ApiExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiExceptions(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }

        
    }
}