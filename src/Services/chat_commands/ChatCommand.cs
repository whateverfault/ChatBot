using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

namespace ChatBot.Services.chat_commands;

public delegate Task CmdActionHandler(ChatCmdArgs chatCmdArgs);

public abstract class ChatCommand {
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
    }
    
    public virtual string GetArgs() {
        return Args;
    }

    public virtual void SetArgs(string args) {
        Args = args;
    }
    
    public virtual string GetDescription() {
        return Description;
    }

    public virtual List<string>? GetAliases() {
        return Aliases;
    }

    public virtual void AddAlias(string alias) {
        Aliases?.Add(alias);
    }
    
    public virtual void RemoveAlias(int index) {
        Aliases?.RemoveAt(index);
    }
    
    public virtual int GetCooldown() {
        return Cooldown;
    }

    public virtual void SetCooldown(int cooldown) {
        Cooldown = cooldown;
    }
    
    public virtual void SetDescription(string desc) {
        Description = desc;
    }
    
    public virtual int GetRestrictionAsInt() {
        return (int)Restriction;
    }

    public virtual void RestrictionNext() {
        Restriction = (Restriction)(((int)Restriction+1)%Enum.GetValues(typeof(Restriction)).Length);
    }
    
    public virtual int GetStateAsInt() {
        return (int)State;
    }

    public virtual void StateNext() {
        State = (State)(((int)State+1)%Enum.GetValues(typeof(State)).Length);
    }
}