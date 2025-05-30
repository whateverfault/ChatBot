using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.chat_commands;

public class SaveData {
    [JsonProperty(PropertyName ="command_identifier")]
    public char CommandIdentifier { get; set; }
    [JsonProperty(PropertyName ="service_state")]
    public State ServiceState{ get; set; }


    public SaveData(State serviceState, char commandIdentifier) {
        ServiceState = serviceState;
        CommandIdentifier = commandIdentifier;
    }
}