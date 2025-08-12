using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.presets;

public class PresetsEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}