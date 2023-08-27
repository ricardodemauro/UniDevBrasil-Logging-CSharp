using Microsoft.AspNetCore.Mvc;

namespace YT.Logging.WebLogging.Advanced.Controllers;

public class LoggingController : Controller
{
    [HttpGet("~/logging")]
    public IActionResult Index([FromServices] ILogger<LoggingController> logger)
    {
        logger.LogInformation("1. Logging from {Action}", nameof(Index));
        logger.LogWarning("2. Logging from {Action}", nameof(Index));

        return Ok(new { Message = "Hello World" });
    }

    [HttpGet("~/logging/from-log-factory")]
    public IActionResult NotTypedLog(
        [FromServices] ILoggerFactory logFactory)
    {
        var logger = logFactory.CreateLogger("SomeCategory");
        logger.LogInformation("1. Logging from {Action}", nameof(Index));
        logger.LogWarning("2. Logging from {Action}", nameof(Index));

        return Ok(new { Message = "Hello World" });
    }
}
