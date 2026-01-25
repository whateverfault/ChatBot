using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.localization;

public class LocalizationEvents : ServiceEvents {
    public override bool Initialized { get; protected set; }
}