using ChatBot.services.interfaces;

namespace ChatBot.services.presets;

public class PresetsEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}