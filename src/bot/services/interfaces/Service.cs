using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.interfaces;

public abstract class Service {
    public abstract string Name { get; }
    public abstract Options Options { get; }


    public virtual void Init() {
        Options.Load();
    }

    public virtual State GetServiceState() {
        return Options.GetState();
    }

    public int GetServiceStateAsInt() {
        return (int)Options.GetState();
    }
    
    public void ServiceStateNext() {
        Options.SetState(Options.ServiceState == State.Enabled ? State.Disabled : State.Enabled);
    }
}