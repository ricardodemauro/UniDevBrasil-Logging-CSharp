using Serilog.Events;

namespace YT.Logging.WebLogging.Advanced.Serilog;

public class SerilogExcludeFilter
{
    internal static LogEventLevel GetLevel(
        HttpContext context,
        double elapsed,
        Exception? exception)
    {
        if (exception is not null) return LogEventLevel.Error;

        if (context.Response.StatusCode > 499) return LogEventLevel.Error;

        return LogEventLevel.Information;
    }
}
