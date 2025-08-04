using ChatBot.services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;

namespace ChatBot.services.chat_commands.Data;

public delegate Task CmdActionHandler(ChatCmdArgs chatCmdArgs);

public abstract class ChatCommand {
    protected static readonly ChatCommandsService ChatCommandsService = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
    
    public abstract int Id { get; protected set; }
    public abstract string Name { get; protected set; }
    public abstract string Args { get; protected set; }
    public abstract string Description { get; protected set; }
    public abstract bool HasIdentifier { get; protected set; }
    public abstract List<string>? Aliases { get; protected set; }
    public abstract int Cooldown { get; protected set; }
    public abstract long LastUsed { get; protected set; }
    public abstract CmdActionHandler Action { get; protected set; }
    public abstract Restriction Restriction { get; protected set; }
    public abstract State State { get; protected set; }


    public virtual string GetName() {
        return Name;
    }

    public virtual void SetName(string name) {
        Name = name;
        ChatCommandsService.Options.Save();
    }
    
    public virtual string GetArgs() {
        return Args;
    }

    public virtual void SetArgs(string args) {
        Args = args;
        ChatCommandsService.Options.Save();
    }
    
    public virtual string GetDescription() {
        return Description;
    }

    public virtual List<string>? GetAliases() {
        return Aliases;
    }

    public virtual void SetAliases(List<string> aliases) {
        Aliases = aliases;
    }
    
    public virtual void AddAlias(string alias) {
        Aliases?.Add(alias);
        ChatCommandsService.Options.Save();
    }
    
    public virtual void RemoveAlias(int index) {
        Aliases?.RemoveAt(index);
        ChatCommandsService.Options.Save();
    }
    
    public virtual int GetCooldown() {
        return Cooldown;
    }

    public virtual void SetCooldown(int cooldown) {
        Cooldown = cooldown;
        ChatCommandsService.Options.Save();
    }
    
    public virtual void SetDescription(string desc) {
        Description = desc;
        ChatCommandsService.Options.Save();
    }
    
    public virtual int GetRestrictionAsInt() {
        return (int)Restriction;
    }

    public virtual void RestrictionNext() {
        Restriction = (Restriction)(((int)Restriction+1)%Enum.GetValues(typeof(Restriction)).Length);
        ChatCommandsService.Options.Save();
    }
    
    public virtual int GetStateAsInt() {
        return (int)State;
    }

    public virtual void StateNext() {
        State = (State)(((int)State+1)%Enum.GetValues(typeof(State)).Length);
        ChatCommandsService.Options.Save();
    }

    public virtual void SetHasIdentifier(bool state) {
        HasIdentifier = state;
        ChatCommandsService.Options.Save();
    }

    public virtual bool GetHasIdentifier() {
        return HasIdentifier;
    }
}