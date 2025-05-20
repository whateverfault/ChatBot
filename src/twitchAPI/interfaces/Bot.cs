using ChatBot.Shared.Handlers;
using ChatBot.Shared.interfaces;
using TwitchLib.Client.Interfaces;

namespace ChatBot.twitchAPI.interfaces;

public abstract class Bot {
    public abstract Options? Options { get; }


    public abstract void Start();

    public abstract void Login();

    public abstract void Enable();

    public abstract void Disable();

    public abstract ErrorCode GetClient(out ITwitchClient client);
    
    public virtual State GetState() {
        return Options!.ServiceState;
    }
    
    public abstract void Toggle();
}