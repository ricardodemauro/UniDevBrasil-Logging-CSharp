using Serilog;
using Serilog.Context;
using Serilog.Formatting.Json;
using YT.Logging.WebLogging;
using YT.Logging.WebLogging.Serilog;

SetUpSerilog();

var builder = WebApplication.CreateBuilder(args);

// 👇 Add Serilog to LogFactory
builder.Host.UseSerilog();

builder.Services.AddControllers();

var app = builder.Build();

// 👇 Calculates Elappsed Time to execute
/*app.UseSerilogRequestLogging(x =>
{
    x.GetLevel = SerilogExcludeFilter.GetLevel;
});*/

// 👇 Calculated Properties using Inline MIddleware
/*app.Use((ctx, next) =>
{
    const string PROTOCOL = "Protocol";
    var httpProtocol = ctx.Request.Protocol;

    const string HTTP_SCHEME = "Scheme";
    var httpScheme = ctx.Request.Scheme;

    using (LogContext.PushProperty(PROTOCOL, httpProtocol))
    using (LogContext.PushProperty(HTTP_SCHEME, httpScheme))
    {
        return next();
    }
});*/

// 👇 Calculated Properties using Full Middleware
/*app.UseEnrichLogMiddleware();*/

app.MapGet("/", () => "Hello World!");

app.MapGet("/delay", async () =>
{
    await Task.Delay(1000 * 2);

    return Results.Ok("Time Ok");
});

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Error(ex, "Unexpected error when executing Host");
    throw;
}
finally
{
    await Serilog.Log.CloseAndFlushAsync();
}

void SetUpSerilog()
{
    var basePath = @"C:\Users\ricardo\source\repos\YT-Logging-Video-1\src\YT.Logging\YT.Logging.WebLogging\";
    var logPath = Path.Combine(basePath, "YT-logging.log");

    Serilog.Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting", Serilog.Events.LogEventLevel.Information)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .WriteTo.Console()
        .WriteTo.File(
            formatter: new JsonFormatter(),
            path: logPath
        )
        .WriteTo.Seq("http://localhost:5341", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
        .Enrich.WithProperty("App", "YT-WebApp-001")
        .CreateBootstrapLogger();
}