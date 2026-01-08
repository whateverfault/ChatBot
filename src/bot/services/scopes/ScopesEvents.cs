using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.scopes.data;

public class ScopesEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}