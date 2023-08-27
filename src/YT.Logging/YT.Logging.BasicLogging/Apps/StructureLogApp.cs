using Serilog;
using Serilog.Formatting.Json;
using System.Diagnostics;

namespace YT.Logging.BasicLogging.Apps;

public class StructureLogApp
{
    public static async Task Run()
    {
        SetUpLogging();

        const string ProgramName = "BasicLoggingApp";

        Log.Information("[Structured] Application {ProgramName} Started at {StartedAt}", ProgramName, DateTime.Now.ToString("yyyy-MM-dd"));

        var timer = Stopwatch.StartNew();
        try
        {
            await ProcessWork();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Handled error");
        }


        timer.Stop();
        Log.Debug("Operation took {Elapsed}", timer.Elapsed);

        Log.Information("[Structured] Application {ProgramName} Ended at {StartedAt}", ProgramName, DateTime.Now.ToString("yyyy-MM-dd"));

        Log.Warning("Finishing the Application...... {ThreadId}", Environment.CurrentManagedThreadId);

        Log.Fatal("Fatal Error");

        FlushLog();
    }

    static async Task ProcessWork()
    {
        for (int i = 0; i < 10; i++)
            Log.Information($"Processing iteration {Guid.NewGuid()}");

        await Task.Delay(TimeSpan.FromSeconds(1));

        throw new Exception("Something bad happened during work");
    }

    static void SetUpLogging()
    {
        var basePath = "C:/Users/ricardo/source/repos/YT-Logging-Video-1/src/YT.Logging/YT.Logging.BasicLogging/samples/";
        var logPath = Path.Combine(basePath, "YT-logging-json.log");

        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                formatter: new JsonFormatter(),
                path: logPath
            )
            .WriteTo.Seq("http://localhost:5341")
            .Enrich.WithEnvironmentName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProperty("App", "YT-App-001")
            .CreateLogger();
    }

    //static void SetUpLogging()
    //{
    //    var basePath = "C:/Users/ricardo/source/repos/YT-Logging-Video-1/src/YT.Logging/YT.Logging.BasicLogging/samples/";
    //    Serilog.Log.Logger = new LoggerConfiguration()
    //        .MinimumLevel.Debug()
    //        .WriteTo.File(
    //            formatter: new JsonFormatter(closingDelimiter: "\n", renderMessage: false),
    //            path: Path.Combine(basePath, "structure-log.log")
    //         )
    //        .WriteTo.Console()
    //        .WriteTo.Seq("http://localhost:5341/")
    //        .Enrich.WithEnvironmentName()
    //        .Enrich.WithProcessName()
    //        .Enrich.WithThreadName()
    //        .Enrich.WithThreadId()
    //        .Enrich.WithProperty("Application", "YT-Logging.Basic")
    //        .CreateLogger();
    //}

    static void FlushLog() => Serilog.Log.CloseAndFlush();
}
