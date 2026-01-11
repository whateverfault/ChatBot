using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class UserInfo {
    public readonly string Id;
    public readonly string UserName;
    public readonly string GlobalName;

    
    public UserInfo(
        [JsonProperty("id")] string id,
        [JsonProperty("username")] string userName,
        [JsonProperty("global_name")] string globalName) {
        Id = id;
        UserName = userName;
        GlobalName = globalName;
    }
}