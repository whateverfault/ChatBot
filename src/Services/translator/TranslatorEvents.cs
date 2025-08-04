using ChatBot.services.interfaces;

namespace ChatBot.services.translator;

public class TranslatorEvents : ServiceEvents{
    public override bool Initialized { get; protected set; }
}