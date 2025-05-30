using ChatBot.bot.interfaces;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.interfaces;

public abstract class Service {
    public abstract string Name { get; }
    public abstract Options Options { get; }


    public virtual ErrorCode Enable(ChatMessage message) {
        if (!PermissionHandler.Handle(Permission.Dev, message)) {
            return ErrorCode.PermDeny;
        }
        if (Options.ServiceState == State.Enabled) {
            return ErrorCode.AlreadyInState;
        }

        Options.SetState(State.Enabled);
        return ErrorCode.None;
    }

    public virtual ErrorCode Disable(ChatMessage message) {
        if (!PermissionHandler.Handle(Permission.Dev, message)) {
            return ErrorCode.PermDeny;
        }
        if (Options.ServiceState == State.Disabled) {
            return ErrorCode.AlreadyInState;
        }

        Options.SetState(State.Disabled);
        return ErrorCode.None;
    }
    
    public virtual void Init(Bot bot) {
        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }
    }

    public virtual State GetServiceState() {
        return Options.GetState();
    }

    public virtual void ToggleService() {
        Options.SetState(Options.ServiceState == State.Enabled ? State.Disabled : State.Enabled);
    }
}