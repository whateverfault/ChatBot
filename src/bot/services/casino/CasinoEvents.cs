using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.casino;

public class CasinoEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}