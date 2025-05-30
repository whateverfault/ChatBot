using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot;

public class SaveData {
    [JsonProperty(PropertyName ="bot_state")]
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName ="username")]
    public string? Username { get; set; }
    [JsonProperty(PropertyName ="channel")]
    public string? Channel { get; set; }
    [JsonProperty(PropertyName ="token")]
    public string? OAuth { get; set; }


    public SaveData() {}

    public SaveData(State serviceState, string username, string oAuth, string channel) {
        ServiceState = serviceState;
        Username = username;
        OAuth = oAuth;
        Channel = channel;
    }
}