using TwitchLib.Client.Models;

namespace ChatBot.Services.interfaces;

public abstract class Service {
    public abstract bool Enabled { get; }

    public abstract ErrorCode Enable(ChatMessage message);
    public abstract ErrorCode Disable();
    
    public abstract void Init();
    public abstract void Kill();
}