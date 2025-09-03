using Newtonsoft.Json;
using TwitchAPI.client.credentials;

namespace ChatBot.bot;

public sealed class SaveData {
    [JsonProperty("credentials")]
    public ConnectionCredentials Credentials { get; set; }


    public SaveData() {
        Credentials = new ConnectionCredentials();
    }
    
    [JsonConstructor]
    public SaveData(ConnectionCredentials credentials) {
        Credentials = credentials;
    }
}