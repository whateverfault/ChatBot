using ChatBot.api.twitch.client;

namespace ChatBot.bot.services.logger;
public class Log {
    public LogLevel Level { get; }
    public DateTime Time { get; }
    public string Message { get; }
    

    public Log(LogLevel level, DateTime time, string message) {
        Level = level;
        Time = time;
        Message = message;
    }
}