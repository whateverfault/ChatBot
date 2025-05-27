using ChatBot.bot.interfaces;

namespace ChatBot.Services.interfaces;

public abstract class ServiceEvents {
    public abstract void Init(Service service, Bot bot);

    public abstract void Subscribe();
}