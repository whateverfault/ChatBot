namespace ChatBot.bot.services.interfaces;

public abstract class ServiceEvents {
    public abstract bool Initialized { get; protected set; }
    
    protected bool Subscribed;


    public virtual void Init(Service service) {
        Kill();
        
        Subscribe();
        Initialized = true;
    }

    public virtual void Kill() {
        if (!Initialized) {
            return;
        }
        
        UnSubscribe();
        Initialized = false;
    }

    protected virtual void Subscribe() {
        UnSubscribe();
        
        Subscribed = true;
    }

    protected virtual void UnSubscribe() {
        Subscribed = false;
    }
}