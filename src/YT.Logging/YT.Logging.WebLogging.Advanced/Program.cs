using Serilog;
using Serilog.Context;
using YT.Logging.WebLogging.Advanced.Serilog;

SetUpSerilog();

var builder = WebApplication.CreateBuilder(args);

// 👇 Add Serilog to LogFactory
builder.Host.UseSerilog();

builder.Services.AddControllers();

var app = builder.Build();

// 👇 Calculates Time to execute
app.UseSerilogRequestLogging(x =>
{
    x.GetLevel = SerilogExcludeFilter.GetLevel;
});

// 👇 Calculated Properties
app.Use((ctx, next) =>
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
});

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
    var configPath = Path.Combine(Directory.GetCurrentDirectory(), "logging.json");
    var config = new ConfigurationBuilder()
        .AddJsonFile(configPath)
        .Build();

    // 👇 Add SelfLog Debug
    Serilog.Debugging.SelfLog.Enable(Console.Error);

    Serilog.Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .Enrich.FromLogContext()
        .CreateBootstrapLogger();
}