using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.translator;

public class TranslatorEvents : ServiceEvents{
    public override bool Initialized { get; protected set; }
}