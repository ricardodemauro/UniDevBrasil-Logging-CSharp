using Microsoft.AspNetCore.Mvc;

namespace YT.Logging.WebLogging.Advanced.Controllers;

public partial class OptimizationsController : Controller
{
    static class Events
    {
        internal static readonly EventId StartingApi = new EventId(100, nameof(StartingApi));
    }

    internal partial class Log
    {
        [LoggerMessage(eventId: 100, level: LogLevel.Information, message: "Starting to track {Caller}", EventName = "StartingApi")]
        internal static partial void StartLog(ILogger logger, string Caller);
    }

    readonly ILogger<OptimizationsController> _logger;

    public OptimizationsController(ILogger<OptimizationsController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("~/optimizations/with-log-level")]
    public IActionResult WithLogLevel()
    {
        //Optimization 1 - Check log Level
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation(Events.StartingApi, "Debug Message");

        //Optminization 2 - Use Source Generators for Logging
        Log.StartLog(_logger, "Caller");

        return Ok(new { Message = "Hello World" });
    }

    [HttpGet("~/optimizations/generators")]
    public IActionResult Generators()
    {
        //Optimization 1 - Check log Level
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation(Events.StartingApi, "Debug Message");

        //Optminization 2 - Use Source Generators for Logging
        Log.StartLog(_logger, "Caller");

        return Ok(new { Message = "Hello World" });
    }
}
