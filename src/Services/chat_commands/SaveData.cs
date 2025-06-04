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
    

    public SaveData(
        [JsonProperty(PropertyName ="service_state")] State serviceState,
        [JsonProperty(PropertyName ="command_identifier")] char commandIdentifier,
        [JsonProperty(PropertyName ="required_role")] Restriction requiredRole
        ) {
        ServiceState = serviceState;
        CommandIdentifier = commandIdentifier;
        RequiredRole = requiredRole;
    }
}