using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ValidationException && exception is not UnauthorizedException)
            {
                _logger.LogError(exception, "Exception occurred during request execution.");
            }

            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                BadRequestException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = statusCode;

            var detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred. Please try again later."
                    : exception.InnerException != null
                        ? $"{exception.Message} -> {exception.InnerException.Message}"
                        : exception.Message;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = GetTitle(statusCode),
                Detail = detail,
                Instance = httpContext.Request.Path
            };


            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static string GetTitle(int statusCode) => statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status409Conflict => "Conflict",
            _ => "Server Error"
        };
    }
}