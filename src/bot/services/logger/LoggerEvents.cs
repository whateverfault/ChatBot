using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.logger;

public class LoggerEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}