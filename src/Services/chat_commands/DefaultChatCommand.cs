using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.chat_commands;

public sealed class DefaultChatCommand : ChatCommand {
    [JsonProperty(PropertyName = "name")]
    public override string Name { get; protected set; }
    [JsonProperty(PropertyName = "args")]
    public override string Args { get; protected set; }
    [JsonProperty(PropertyName = "description")]
    public override string Description { get; protected set; }
    [JsonProperty(PropertyName = "cooldown")]
    public override int Cooldown { get; protected set; }
    [JsonProperty(PropertyName = "last_used")]
    public override long LastUsed { get; protected set; }
    [JsonIgnore]
    public override CmdActionHandler? Action { get; protected set; }

    [JsonProperty(PropertyName = "restriction")]
    public override Restriction Restriction { get; protected set; }
    [JsonProperty(PropertyName = "state")]
    public override State State { get; protected set; }
    
    
    public DefaultChatCommand(
        string name,
        string args,
        string description,
        CmdActionHandler action,
        Restriction restriction,
        int cooldown = 1, 
        long lastUsed = 0,
        State state = State.Enabled) {
        Name = name;
        Args = args;
        Description = description;
        Action = action;
        Restriction = restriction;
        Cooldown = cooldown;
        LastUsed = lastUsed;
        State = state;
    }
    
    [JsonConstructor]
    public DefaultChatCommand(
        [JsonProperty(PropertyName = "name")] string name,
        [JsonProperty(PropertyName = "args")] string args,
        [JsonProperty(PropertyName = "description")] string description,
        [JsonProperty(PropertyName = "restriction")] Restriction restriction, 
        [JsonProperty(PropertyName = "cooldown")] int cooldown = 1, 
        [JsonProperty(PropertyName = "last_used")] long lastUsed = 0,
        [JsonProperty(PropertyName = "state")] State state = State.Enabled) {
        Name = name;
        Args = args;
        Description = description;
        foreach (var cmd in CommandsList.DefaultsCommands) {
            if (!cmd.Name.Equals(name)) continue;

            Action = cmd.Action;
            break;
        }
        Restriction = restriction;
        Cooldown = cooldown;
        LastUsed = lastUsed;
        State = state;
    }
}