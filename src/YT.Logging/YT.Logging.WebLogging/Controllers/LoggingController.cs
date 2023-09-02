using Microsoft.AspNetCore.Mvc;

namespace YT.Logging.WebLogging.Controllers;

public class LoggingController : Controller
{
    readonly ILogger<LoggingController> _logger;

    public LoggingController(ILogger<LoggingController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("~/logging")]
    public IActionResult Default()
    {
        _logger.LogInformation("1. Logging from {RequestPath}", HttpContext.Request.Path);
        _logger.LogWarning("2. Logging from {RequestPath}", HttpContext.Request.Path);

        return Ok(new { Message = "Hello World" });
    }

    [HttpGet("~/logging/log-factory")]
    public IActionResult LogFactory(
        [FromServices] ILoggerFactory logFactory)
    {
        var logger = logFactory.CreateLogger<LoggingController>();

        logger.LogInformation("1. Logging from {RequestPath}", HttpContext.Request.Path);
        logger.LogWarning("2. Logging from {RequestPath}", HttpContext.Request.Path);

        return Ok(new { Message = "Hello World" });
    }
}
