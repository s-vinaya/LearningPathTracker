namespace learning_path_tracker.Application.Logging;

public class FileLogger
{
    private readonly string _logPath;
    private static readonly object _lock = new object();

    public FileLogger(string logPath)
    {
        _logPath = logPath;
        Directory.CreateDirectory(Path.GetDirectoryName(_logPath)!);
    }

    public void Log(string level, string layer, string message, Exception? exception = null)
    {
        lock (_lock)
        {
            var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] [{layer}] {message}";
            if (exception != null)
                logEntry += $"\nException: {exception.Message}\nStackTrace: {exception.StackTrace}";
            
            File.AppendAllText(_logPath, logEntry + Environment.NewLine);
        }
    }

    public void LogInfo(string layer, string message) => Log("INFO", layer, message);
    public void LogError(string layer, string message, Exception? exception = null) => Log("ERROR", layer, message, exception);
    public void LogWarning(string layer, string message) => Log("WARNING", layer, message);
}
