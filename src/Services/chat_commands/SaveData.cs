using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.services.chat_commands;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState{ get; set; }
    [JsonProperty("command_identifier")]
    public char CommandIdentifier { get; set; }

    [JsonProperty("verbose_state")]
    public State VerboseState { get; set; }

    [JsonProperty("base_title")]
    public string BaseTitle { get; set; } = string.Empty;

    [JsonProperty("swip")]
    public State SendWhisperIfPossible { get; set; }


    public SaveData() {}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("command_identifier")] char commandIdentifier,
        [JsonProperty("verbose_state")] State verboseState,
        [JsonProperty("base_title")] string baseTitle,
        [JsonProperty("swip")] State sendWhisperIfPossible) {
        ServiceState = serviceState;
        CommandIdentifier = commandIdentifier;
        VerboseState = verboseState;
        BaseTitle = baseTitle;
        SendWhisperIfPossible = sendWhisperIfPossible;
    }
}