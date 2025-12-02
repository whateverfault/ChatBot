using TwitchAPI.client;

namespace ChatBot.bot.services.logger.data;
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