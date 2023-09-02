using Serilog;
using Serilog.Context;
using Serilog.Formatting.Json;
using System.Security.Claims;
using YT.Logging.WebLogging;
using YT.Logging.WebLogging.Serilog;

SetUpSerilog();

var builder = WebApplication.CreateBuilder(args);

// 👇 Add Serilog to LogFactory
builder.Host.UseSerilog();

builder.Services.AddControllers();

var app = builder.Build();

// 👇 Calculates Elappsed Time to execute
app.UseSerilogRequestLogging(x =>
{
    x.GetLevel = SerilogExcludeFilter.GetLevel;
});

// 👇 Calculated Properties using Inline Middleware
//app.Use((httpContext, next) =>
//{
//    const string PROTOCOL = "Protocol";
//    var httpProtocol = httpContext.Request.Protocol;

//    const string HTTP_SCHEME = "Scheme";
//    var httpScheme = httpContext.Request.Scheme;

//    //var userIdClaim = httpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);

//    using (LogContext.PushProperty(PROTOCOL, httpProtocol))
//    //using (LogContext.PushProperty("UserId", userIdClaim?.Value ?? "unknow"))
//    using (LogContext.PushProperty(HTTP_SCHEME, httpScheme))
//    {
//        return next();
//    }
//});

// 👇 Calculated Properties using Full Middleware
app.UseEnrichLogMiddleware();

app.MapGet("/", () => "Hello World!");

app.MapGet("/minimal/logging", (HttpContext ctx) =>
{
    var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Hello from logging");

    return "Hello World!";
});

app.MapGet("/minimal/delay", async (ILogger<Program> logger) =>
{
    logger.LogInformation("Calling api method {ApiMethodName}", "/delay");

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
    Serilog.Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting", Serilog.Events.LogEventLevel.Information)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        //.MinimumLevel.Override("YT.Logging.WebLogging", Serilog.Events.LogEventLevel.Information)
        .WriteTo.Console()
        .WriteTo.Seq(
            "http://localhost:5341",
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
        )
        .Enrich.WithProperty("App", "YT-WebApp-002")
        .Enrich.FromLogContext()
        .CreateBootstrapLogger();
}