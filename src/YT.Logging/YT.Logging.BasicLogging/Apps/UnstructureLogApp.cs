using System.Diagnostics;
using System.Reflection;

namespace YT.Logging.BasicLogging.Apps;

public class UnstructureLogApp
{
    class Log
    {
        const string _path = @"C:\Users\ricardo\source\repos\YT-Logging-Video-1\src\YT.Logging\YT.Logging.BasicLogging\samples\";

        static readonly StreamWriter _writer;

        static Log()
        {
            var fs = File.OpenWrite(Path.Combine(_path, "unstructure-log.log"));
            _writer = new StreamWriter(fs);
        }

        internal static void Information(string message)
        {
            WriteLog("Information", message);
        }

        internal static void Debug(string message)
        {
            WriteLog("Debug", message);
        }

        internal static void Warning(string message)
        {
            WriteLog("Warning", message);
        }

        internal static void Error(string message)
        {
            WriteLog("Warning", message);
        }

        static void WriteLog(string level, string message)
        {
            _writer.WriteLine($"{level} {message}");
        }

        internal static void Flush()
        {
            _writer.Flush();
            _writer.Close();
        }
    }

    public static async Task Run()
    {
        const string ProgramName = "BasicLoggingApp";

        Log.Information($"[Non Structured] Application {ProgramName} Started at: {DateTime.Now:yyyy-MM-dd}");

        var timer = Stopwatch.StartNew();
        try
        {
            await ProcessWork();
        }
        catch (Exception ex)
        {
            Log.Error($"Handled error. {ex.Message}");
        }

        timer.Stop();

        Log.Debug($"Operation took {timer.Elapsed}");
        Log.Information($"[Non Structured] Application {ProgramName} Ended at: {DateTime.Now:yyyy-MM-dd}");

        Log.Warning($"Finishing the Application...... {Environment.CurrentManagedThreadId}");

        Log.Flush();
    }

    static async Task ProcessWork()
    {
        for (int i = 0; i < 150; i++)
            Log.Information($"Processing iteration {i}");

        await Task.Delay(TimeSpan.FromSeconds(1));

        throw new Exception("Something bad happened during work");
    }
}
