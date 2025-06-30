using ChatBot.Services.chat_commands.Data;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.chat_commands;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState{ get; set; }
    [JsonProperty("command_identifier")]
    public char CommandIdentifier { get; set; }

    [JsonProperty("verbose_state")]
    public State VerboseState { get; set; }
    
    [JsonProperty("custom_commands")]
    public List<CustomChatCommand> CustomCmds { get; set; } = [];
    
    [JsonProperty("default_commands")]
    public List<DefaultChatCommand> DefaultCmds { get; set; } = [];

    [JsonProperty("base_title")]
    public string BaseTitle { get; set; } = string.Empty;


    public SaveData() {}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("command_identifier")] char commandIdentifier,
        [JsonProperty("verbose_state")] State verboseState,
        [JsonProperty("custom_commands")] List<CustomChatCommand> customCmds,
        [JsonProperty("default_commands")] List<DefaultChatCommand> defaultCmds,
        [JsonProperty("base_title")] string baseTitle) {
        ServiceState = serviceState;
        CommandIdentifier = commandIdentifier;
        VerboseState = verboseState;
        CustomCmds = customCmds;
        DefaultCmds = defaultCmds;
        BaseTitle = baseTitle;
    }
}