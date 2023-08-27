using Serilog.Context;

namespace YT.Logging.WebLogging;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class EnrichLogMiddleware
{
    private readonly RequestDelegate _next;

    public EnrichLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        const string PROTOCOL = "Protocol";
        var httpProtocol = httpContext.Request.Protocol;

        const string HTTP_SCHEME = "Scheme";
        var httpScheme = httpContext.Request.Scheme;

        using (LogContext.PushProperty(PROTOCOL, httpProtocol))
        using (LogContext.PushProperty(HTTP_SCHEME, httpScheme))
        {
            return _next(httpContext);
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class EnrichLogMiddlewareExtensions
{
    public static IApplicationBuilder UseEnrichLogMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<EnrichLogMiddleware>();
    }
}
