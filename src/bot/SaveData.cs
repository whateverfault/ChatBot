using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot;

public sealed class SaveData {
    [JsonProperty(PropertyName ="bot_state")]
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName ="username")]
    public string? Username { get; set; }
    [JsonProperty(PropertyName ="channel")]
    public string? Channel { get; set; }
    [JsonProperty(PropertyName ="token")]
    public string? OAuth { get; set; }
    [JsonProperty(PropertyName ="broadcaster_token")]
    public string? BroadcasterOAuth { get; set; }
    [JsonProperty(PropertyName ="client_id")]
    public string? ClientId { get; set; }


    public SaveData() {}
    
    public SaveData(
        [JsonProperty(PropertyName = "bot_state")] State serviceState,
        [JsonProperty(PropertyName = "username")] string username,
        [JsonProperty(PropertyName = "channel")] string channel,
        [JsonProperty(PropertyName = "token")] string oAuth,
        [JsonProperty(PropertyName ="broadcaster_token")] string? broadcasterOAuth,
        [JsonProperty(PropertyName = "client_id")] string clientId) {
        ServiceState = serviceState;
        Username = username;
        OAuth = oAuth;
        Channel = channel;
        ClientId = clientId;
        BroadcasterOAuth = broadcasterOAuth;
    }
}