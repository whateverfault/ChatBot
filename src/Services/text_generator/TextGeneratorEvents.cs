using ChatBot.services.interfaces;

namespace ChatBot.services.text_generator;

public class TextGeneratorEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}