using ChatBot.bot.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_commands.data;

public delegate Task CmdActionHandler(ChatCmdArgs chatCmdArgs);
public delegate string DynamicStringHandler(ChatCommand cmd);


public abstract class ChatCommand {
    protected static readonly ChatCommandsService ChatCommandsService = (ChatCommandsService)Services.Get(ServiceId.ChatCommands);
    
    [JsonProperty("id")]
    public int Id { get; protected set; }
    
    [JsonProperty("name")]
    public string Name { get; protected set; }
    
    [JsonProperty("args")]
    public string Args { get; protected set; }

    [JsonProperty("description")]
    public string StaticDescription;
    
    [JsonIgnore]
    private DynamicStringHandler? DynamicDescription { get; }
    
    [JsonIgnore]
    public string Description {
        get => DynamicDescription == null
               ? StaticDescription
               : DynamicDescription.Invoke(this);
        private set => StaticDescription = value;
    }
    
    [JsonProperty("has_identifier")]
    public bool HasIdentifier { get; protected set; }
    
    [JsonProperty("aliases")]
    public List<string> Aliases { get; protected set; }
    
    [JsonProperty("cooldown")]
    public long Cooldown { get; protected set; }
    
    [JsonProperty("cooldown_per_user")]
    public bool CooldownPerUser { get; protected set; }
    
    [JsonProperty("cooldown_users")]
    public Dictionary<string, CooldownUser> CooldownUsers { get; protected set; }
    
    [JsonProperty("last_used")]
    public long LastUsed { get; private set; }
    
    [JsonIgnore]
    public CmdActionHandler Action { get; }
    
    [JsonProperty("restriction")]
    public Restriction Restriction { get; protected set; }
    
    [JsonProperty("state")]
    public State State { get; protected set; }
    
    
    protected ChatCommand(
        int id,
        string name,
        string args,
        string desc,
        bool hasIdentifier,
        List<string> aliases,
        int cooldown,
        bool cooldownPerUser,
        Dictionary<string, CooldownUser> cooldownUsers,
        long lastUsed,
        CmdActionHandler action,
        Restriction restriction,
        State state,
        DynamicStringHandler? dynamicDescription = null) {
        Id = id;
        Name = name;
        Args = args;
        StaticDescription = desc;
        HasIdentifier = hasIdentifier;
        Aliases = aliases;
        Cooldown = cooldown;
        CooldownPerUser = cooldownPerUser;
        CooldownUsers = cooldownUsers;
        LastUsed = lastUsed;
        Action = action;
        Restriction = restriction;
        State = state;

        DynamicDescription = dynamicDescription;
    }

    public string GetName() {
        return Name;
    }

    public void SetName(string name) {
        Name = name;
        ChatCommandsService.Options.Save();
    }
    
    public string GetArgs() {
        return Args;
    }

    public void SetArgs(string args) {
        Args = args;
        ChatCommandsService.Options.Save();
    }
    
    public string GetDescription() {
        return Description;
    }
    
    public void SetDescription(string desc) {
        Description = desc;
        ChatCommandsService.Options.Save();
    }

    public List<string> GetAliases() {
        return Aliases;
    }

    public void SetAliases(List<string> aliases) {
        Aliases = aliases;
        ChatCommandsService.Options.Save();
    }
    
    public long GetCooldown() {
        return Cooldown;
    }

    public void SetCooldown(long cooldown) {
        Cooldown = cooldown;
        ChatCommandsService.Options.Save();
    }

    public bool GetCooldownPerUser() {
        return CooldownPerUser;
    }

    public void SetCooldownPerUser(bool value) {
        CooldownPerUser = value;
        ChatCommandsService.Options.Save();
    }
    
    public void AddCooldownUser(string userId) {
        CooldownUsers.Remove(userId);
        CooldownUsers.Add(userId, new CooldownUser(userId));
        ChatCommandsService.Options.Save();
    }
    
    public void SetLastUsed(long time) {
        LastUsed = time;
        ChatCommandsService.Options.Save();
    }
    
    public int GetRestrictionAsInt() {
        return (int)Restriction;
    }

    public void RestrictionNext() {
        Restriction = (Restriction)(((int)Restriction+1)%Enum.GetValues(typeof(Restriction)).Length);
        ChatCommandsService.Options.Save();
    }
    
    public int GetStateAsInt() {
        return (int)State;
    }

    public void StateNext() {
        State = (State)(((int)State+1)%Enum.GetValues(typeof(State)).Length);
        ChatCommandsService.Options.Save();
    }

    public void SetHasIdentifier(bool state) {
        HasIdentifier = state;
        ChatCommandsService.Options.Save();
    }

    public bool GetHasIdentifier() {
        return HasIdentifier;
    }
}