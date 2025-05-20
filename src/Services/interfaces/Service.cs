using ChatBot.Shared.Handlers;
using ChatBot.Shared.interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.interfaces;

public abstract class Service {
    public abstract Options Options { get; }

    public abstract ErrorCode Enable(ChatMessage message);
    public abstract ErrorCode Disable(ChatMessage message);
    
    
    public abstract void Init();

    public virtual void Kill() {
        Options.Save();
    }
    
    public abstract State GetServiceState();

    public abstract void ToggleService();
}