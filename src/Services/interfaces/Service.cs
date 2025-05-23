using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.twitchAPI.interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.interfaces;

public abstract class Service {
    public abstract string Name { get; }
    public abstract Options Options { get; }

    
    public virtual ErrorCode Enable(ChatMessage message) {
        if (!PermissionHandler.Handle(Permission.Dev, message)) {
            return ErrorCode.PermDeny;
        }
        if (Options.State == State.Enabled) {
            return ErrorCode.AlreadyInState;
        }

        Options.SetState(State.Enabled);
        return ErrorCode.None;
    }

    public virtual ErrorCode Disable(ChatMessage message) {
        if (!PermissionHandler.Handle(Permission.Dev, message)) {
            return ErrorCode.PermDeny;
        }
        if (Options.State == State.Disabled) {
            return ErrorCode.AlreadyInState;
        }

        Options.SetState(State.Disabled);
        return ErrorCode.None;
    }


    public abstract void Init(Bot bot);

    public abstract State GetServiceState();

    public abstract void ToggleService();
}