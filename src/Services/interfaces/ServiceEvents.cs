using ChatBot.bot.interfaces;

namespace ChatBot.Services.interfaces;

public abstract class ServiceEvents {
    protected bool subscribed;
    
    
    public abstract void Init(Service service, Bot bot);

    public virtual void Subscribe() {
        subscribed = true;
    }
}