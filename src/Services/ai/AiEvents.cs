using ChatBot.services.interfaces;

namespace ChatBot.services.ai;

public class AiEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}