using ProblemDetails;

namespace ElGuerre.Taskin.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseTasking(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();

        return app;
    }
}