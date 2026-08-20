using System.Net;
using Wcs.Common.Exceptions;

namespace Wcs.Cms.Middlewares;

public class GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger) : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var statusCode = context.Response.StatusCode;

            if (ex is NotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
            }
            else if (ex is ValidationException)
            {
                statusCode = (int)HttpStatusCode.UnprocessableEntity; // 422
            }

            var response = new
            {
                status = statusCode,
                message = ex.Message,
                trace = ex.StackTrace ?? string.Empty,
                errors = (List<string>?)null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
