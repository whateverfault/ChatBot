using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.ai;

public class AiEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}