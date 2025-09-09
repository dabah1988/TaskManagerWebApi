using Microsoft.IdentityModel.Tokens;

namespace TaskManager.UI.Exceptions
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }
        public static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = StatusCodes.Status500InternalServerError; // 500 if unexpected
            if (exception is SecurityTokenExpiredException) code = StatusCodes.Status401Unauthorized;
            else if (exception is UnauthorizedAccessException) code = StatusCodes.Status403Forbidden;
            else if (exception is ArgumentException) code = StatusCodes.Status400BadRequest;
            // Add more custom exceptions and corresponding status codes as needed
            var result = System.Text.Json.JsonSerializer.Serialize(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            return context.Response.WriteAsync(result);
        }
    }
}