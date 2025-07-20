using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.chat_commands.Data;

public sealed class DefaultChatCommand : ChatCommand {
    [JsonProperty(PropertyName = "id")]
    public override int Id { get; protected set; }
    [JsonProperty(PropertyName = "name")]
    public override string Name { get; protected set; }
    [JsonProperty(PropertyName = "args")]
    public override string Args { get; protected set; }
    [JsonProperty(PropertyName = "description")]
    public override string Description { get; protected set; }
    [JsonProperty(PropertyName = "aliases")]
    public override List<string>? Aliases { get; protected set; }
    [JsonProperty(PropertyName = "cooldown")]
    public override int Cooldown { get; protected set; }
    [JsonIgnore]
    public override long LastUsed { get; protected set; }
    [JsonIgnore]
    public override CmdActionHandler? Action { get; protected set; }

    [JsonProperty(PropertyName = "restriction")]
    public override Restriction Restriction { get; protected set; }
    [JsonProperty(PropertyName = "state")]
    public override State State { get; protected set; }
    
    
    public DefaultChatCommand(
        int id,
        string name,
        string args,
        string description,
        CmdActionHandler action,
        Restriction restriction,
        int cooldown = 1, 
        long lastUsed = 0,
        List<string>? aliases = null,
        State state = State.Enabled) {
        Id = id;
        Name = name.Replace(" ", "");
        Args = args;
        Description = description;
        Action = action;
        Restriction = restriction;
        Cooldown = cooldown;
        LastUsed = lastUsed;
        Aliases = aliases;
        State = state;
    }
    
    [JsonConstructor]
    public DefaultChatCommand(
        [JsonProperty(PropertyName = "id")] int id,
        [JsonProperty(PropertyName = "name")] string name,
        [JsonProperty(PropertyName = "args")] string args,
        [JsonProperty(PropertyName = "description")] string description,
        [JsonProperty(PropertyName = "last_used")] List<string>? aliases,
        [JsonProperty(PropertyName = "restriction")] Restriction restriction,
        [JsonProperty(PropertyName = "cooldown")] int cooldown,
        [JsonProperty(PropertyName = "state")] State state) {
        Id = id;
        Name = name.Replace(" ", "");
        Args = args;
        Description = description;
        foreach (var cmd in CommandsList.DefaultsCommands) {
            if (cmd.Id != Id) continue;

            Action = cmd.Action;
            break;
        }
        Restriction = restriction;
        Cooldown = cooldown;
        LastUsed = 0;
        Aliases = aliases;
        State = state;
    }
}