using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class UserInfo {
    public readonly string Id;
    public readonly string Username;
    public readonly string GlobalName;

    
    public UserInfo(
        [JsonProperty("id")] string id,
        [JsonProperty("username")] string username,
        [JsonProperty("global_name")] string globalName) {
        Id = id;
        Username = username;
        GlobalName = globalName;
    }
}