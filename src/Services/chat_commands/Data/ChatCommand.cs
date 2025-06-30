using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

namespace ChatBot.Services.chat_commands.Data;

public delegate Task CmdActionHandler(ChatCmdArgs chatCmdArgs);

public abstract class ChatCommand {
    protected static ChatCommandsService chatCommandsService = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
    
    public abstract string Name { get; protected set; }
    public abstract string Args { get; protected set; }
    public abstract string Description { get; protected set; }
    public abstract List<string>? Aliases { get; protected set; }
    public abstract int Cooldown { get; protected set; }
    public abstract long LastUsed { get; protected set; }
    public abstract CmdActionHandler? Action { get; protected set; }
    public abstract Restriction Restriction { get; protected set; }
    public abstract State State { get; protected set; }


    public virtual string GetName() {
        return Name;
    }

    public virtual void SetName(string name) {
        Name = name;
        chatCommandsService.Options.Save();
    }
    
    public virtual string GetArgs() {
        return Args;
    }

    public virtual void SetArgs(string args) {
        Args = args;
        chatCommandsService.Options.Save();
    }
    
    public virtual string GetDescription() {
        return Description;
    }

    public virtual List<string>? GetAliases() {
        return Aliases;
    }

    public virtual void AddAlias(string alias) {
        Aliases?.Add(alias);
        chatCommandsService.Options.Save();
    }
    
    public virtual void RemoveAlias(int index) {
        Aliases?.RemoveAt(index);
        chatCommandsService.Options.Save();
    }
    
    public virtual int GetCooldown() {
        return Cooldown;
    }

    public virtual void SetCooldown(int cooldown) {
        Cooldown = cooldown;
        chatCommandsService.Options.Save();
    }
    
    public virtual void SetDescription(string desc) {
        Description = desc;
        chatCommandsService.Options.Save();
    }
    
    public virtual int GetRestrictionAsInt() {
        return (int)Restriction;
    }

    public virtual void RestrictionNext() {
        Restriction = (Restriction)(((int)Restriction+1)%Enum.GetValues(typeof(Restriction)).Length);
        chatCommandsService.Options.Save();
    }
    
    public virtual int GetStateAsInt() {
        return (int)State;
    }

    public virtual void StateNext() {
        State = (State)(((int)State+1)%Enum.GetValues(typeof(State)).Length);
        chatCommandsService.Options.Save();
    }
}