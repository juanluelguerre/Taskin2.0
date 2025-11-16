using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ElGuerre.Taskin.Api.Observability;

/// <summary>
/// Constants for OpenTelemetry instrumentation
/// </summary>
public static class TelemetryConstants
{
    /// <summary>
    /// Activity source name for distributed tracing
    /// </summary>
    public const string ActivitySourceName = "Taskin.Api";

    /// <summary>
    /// Meter name for custom metrics
    /// </summary>
    public const string MeterName = "Taskin.Api";

    /// <summary>
    /// Service version
    /// </summary>
    public const string ServiceVersion = "1.0.0";

    /// <summary>
    /// Activity source instance for creating activities (spans)
    /// </summary>
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, ServiceVersion);

    /// <summary>
    /// Meter instance for creating metrics
    /// </summary>
    public static readonly Meter Meter = new(MeterName, ServiceVersion);
}
