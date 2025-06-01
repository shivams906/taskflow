// Middleware/ErrorHandlingMiddleware.cs
using System.Text.Json;

namespace TaskFlowAPI.Middleware
{
    // Middleware/ErrorHandlingMiddleware.cs
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex switch
                {
                    UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                    InvalidOperationException => StatusCodes.Status404NotFound,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    ArgumentException => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };

                var result = JsonSerializer.Serialize(new
                {
                    error = ex.GetType().Name,
                    message = ex.Message
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}