using TC_Backend.Exceptions;

namespace TC_Backend.Middleware
{
    public class ExceptHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptHandlingMiddleware> _logger;

        public ExceptHandlingMiddleware(RequestDelegate next, ILogger<ExceptHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Map exceptions to HTTP status codes and messages
            var (statusCode, message) = exception switch
            {
                UserAlreadyExistsException =>
                    (StatusCodes.Status409Conflict, exception.Message),

                UserCreationException =>
                    (StatusCodes.Status400BadRequest, exception.Message),

                RoleNotFoundException =>
                    (StatusCodes.Status404NotFound, exception.Message),

                KeyNotFoundException =>
                    (StatusCodes.Status404NotFound, exception.Message),

                CloudinaryUploadException =>
                    (StatusCodes.Status400BadRequest, exception.Message),

                CloudinaryDeleteException =>
                    (StatusCodes.Status400BadRequest, exception.Message),

                CompanyListRepoExcep =>
                    (StatusCodes.Status500InternalServerError, exception.Message),

                CompanyListServiceExcep =>
                    (StatusCodes.Status400BadRequest, exception.Message),

                InvalidOperationException =>
                    (StatusCodes.Status400BadRequest, exception.Message),

                UnauthorizedAccessException =>
                    (StatusCodes.Status401Unauthorized, "Unauthorized access."),

                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                Error = message,
                StatusCode = statusCode
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
