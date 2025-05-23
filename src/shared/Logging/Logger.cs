namespace ChatBot.shared.Logging;

public delegate void LogHandler(Log log);

public static class Logger {
    private static readonly List<Log> _logs = [];

    public static event LogHandler? OnLog;


    public static void Log(LogLevel level, string message) {
        var log = new Log(level, DateTime.Now, message);

        _logs.Add(log);
        OnLog?.Invoke(log);
    }
}