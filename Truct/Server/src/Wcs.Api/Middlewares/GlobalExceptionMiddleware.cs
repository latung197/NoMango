using System.Text.Json;
using Wcs.Api.Controllers.DTOs;
using Wcs.Common.Exceptions;

namespace Wcs.Api.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex) when (ex is StationNotAvailableException or InvalidOperationException)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status400BadRequest,
                BaseResponse.ErrorResult(ex.Message, "Bad Request"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                BaseResponse.ErrorResult("Internal server error", "INTERNAL_SERVER_ERROR"));
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, BaseResponse response)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
