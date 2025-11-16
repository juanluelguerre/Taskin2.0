using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElGuerre.Taskin.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly ActivitySource ActivitySource = new("Taskin.Application");

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Create an activity (span) for distributed tracing
        using var activity = ActivitySource.StartActivity($"MediatR.{requestName}", ActivityKind.Internal);

        // Add tags to the activity for better observability
        activity?.SetTag("messaging.system", "MediatR");
        activity?.SetTag("messaging.operation", "process");
        activity?.SetTag("messaging.destination", requestName);

        try
        {
            logger.LogInformation("Handling {RequestName} with payload: {@Request}",
                requestName, request);

            var response = await next();

            activity?.SetTag("success", true);
            logger.LogInformation("Handled {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            activity?.SetTag("success", false);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(ex, "Error handling {RequestName}", requestName);
            throw;
        }
    }
}