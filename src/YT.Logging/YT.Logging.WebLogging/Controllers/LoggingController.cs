using Microsoft.AspNetCore.Mvc;

namespace YT.Logging.WebLogging.Controllers;

public class LoggingController : Controller
{
    static class Events
    {
        internal static readonly EventId EventOne = new EventId(100, nameof(EventOne));
        internal static readonly EventId EventTwo = new EventId(101, nameof(EventTwo));
    }

    [HttpGet("~/logging")]
    public IActionResult Index(
        [FromServices] ILogger<LoggingController> logger)
    {
        logger.LogInformation("1. Logging from {Action}", nameof(Index));
        logger.LogWarning("2. Logging from {Action}", nameof(Index));

        return Ok(new { Message = "Hello World" });
    }

    [HttpGet("~/logging/events")]
    public IActionResult WithEvent(
        [FromServices] ILogger<LoggingController> logger)
    {
        logger.LogInformation(Events.EventOne, "1. Logging from {Action}", nameof(Index));
        logger.LogWarning(Events.EventTwo, "2. Logging from {Action}", nameof(Index));

        return Ok(new { Message = "Hello World" });
    }
}
