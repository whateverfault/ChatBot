using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.text_generator;

public class TextGeneratorEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}