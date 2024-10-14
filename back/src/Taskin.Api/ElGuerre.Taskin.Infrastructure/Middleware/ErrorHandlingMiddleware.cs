using System.Text.Json;
using ElGuerre.Taskin.Application.Errors;
using ElGuerre.Taskin.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ElGuerre.Taskin.Infrastructure.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await (ex is TaskinExceptionBase exception
                ? HandleTaskinException(context, exception)
                : HandleUnexpectedException(context, ex));
        }
    }

    private static Task HandleTaskinException(HttpContext context,
        TaskinExceptionBase exception)
    {
        context.Response.Clear();
        context.Response.ContentType = "application/json";

        var status = GetStatusCode(exception);

        return status is not null
            ? CreateTaskinResponse(context, status.Value, exception)
            : HandleUnexpectedException(context, exception);
    }

    private static int? GetStatusCode(Exception exception) => exception switch
    {
        UnauthorizedException => StatusCodes.Status401Unauthorized,
        ForbiddenEntityException => StatusCodes.Status403Forbidden,
        EntityNotFoundException => StatusCodes.Status404NotFound,
        not null when exception.GetType() == typeof(EntityNotFoundException<>) =>
            StatusCodes.Status404NotFound,
        EntityAlreadyExistsException => StatusCodes.Status409Conflict,
        KeyAlreadyExistsException => StatusCodes.Status409Conflict,
        not null when exception.GetType() == typeof(EntityAlreadyExistsException<>) =>
            StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private static Task HandleUnexpectedException(HttpContext context, Exception exception)
    {
        var error = new Error("An error occurred during action handling", exception.Message);
        Log.Error(exception, error.Description);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsync(JsonSerializer.Serialize(error));
    }

    private static Task CreateTaskinResponse(HttpContext context, int status,
        ITaskinException exception)
    {
        var options =
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        LogException(exception);
        context.Response.StatusCode = status;
        return context.Response.WriteAsync(
            JsonSerializer.Serialize(
                new Error(exception.Code, exception.Message, [.. exception.Values]), options));
    }

    private static void LogException(ITaskinException exception)
    {
        switch (exception)
        {
            case BusinessException businessException:
                Log.Information(
                    $"Business Exception: {businessException.Code} - {businessException.Message}");
                break;
            default:
                Log.Information(exception.Message);
                break;
        }
    }
}