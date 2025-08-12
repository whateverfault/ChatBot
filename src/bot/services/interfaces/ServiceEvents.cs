namespace ChatBot.bot.services.interfaces;

public abstract class ServiceEvents {
    public abstract bool Initialized { get; protected set; }
    
    protected bool Subscribed;


    public virtual void Init(Service service) {
        Subscribe();
        Initialized = true;
    }

    public virtual void Kill() {
        UnSubscribe();
        Initialized = false;
    }

    protected virtual void Subscribe() {
        Subscribed = true;
    }

    protected virtual void UnSubscribe() {
        Subscribed = false;
    }
}