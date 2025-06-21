using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.chat_commands;

public class SaveData {
    [JsonProperty(PropertyName ="service_state")]
    public State ServiceState{ get; set; }
    [JsonProperty(PropertyName ="command_identifier")]
    public char CommandIdentifier { get; set; }
    [JsonProperty(PropertyName ="required_role")]
    public Restriction RequiredRole { get; set; }
    [JsonProperty(PropertyName ="pattern_index")]
    public int ModActionIndex { get; set; }
    [JsonProperty(PropertyName ="cooldown")]
    public int Cooldown { get; set; }
    [JsonProperty(PropertyName = "verbose_state")]
    public State VerboseState { get; set; }
    
    [JsonProperty(PropertyName = "custom_commands")]
    public List<CustomChatCommand> CustomCmds { get; set; } = [];
    
    [JsonProperty(PropertyName = "default_commands")]
    public List<DefaultChatCommand> DefaultCmds { get; set; } = [];


    public SaveData() {}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName ="service_state")] State serviceState,
        [JsonProperty(PropertyName ="command_identifier")] char commandIdentifier,
        [JsonProperty(PropertyName ="required_role")] Restriction requiredRole,
        [JsonProperty(PropertyName ="pattern_index")] int modActionIndex,
        [JsonProperty(PropertyName ="cooldown")] int cooldown,
        [JsonProperty(PropertyName = "verbose_state")] State verboseState,
        [JsonProperty(PropertyName = "custom_commands")] List<CustomChatCommand> customCmds,
        [JsonProperty(PropertyName = "default_commands")] List<DefaultChatCommand> defaultCmds) {
        ServiceState = serviceState;
        CommandIdentifier = commandIdentifier;
        RequiredRole = requiredRole;
        ModActionIndex = modActionIndex;
        Cooldown = cooldown;
        VerboseState = verboseState;
        CustomCmds = customCmds;
        DefaultCmds = defaultCmds;
    }
}