using Newtonsoft.Json;

namespace ChatBot.bot.utils.GD.AREDL.Data;

public class UserInfo {
    public string id;
    public string username;
    public string globalName;

    
    public UserInfo(
        [JsonProperty(PropertyName = "id")] string id,
        [JsonProperty(PropertyName = "username")] string username,
        [JsonProperty(PropertyName = "global_name")] string globalName) {
        this.id = id;
        this.username = username;
        this.globalName = globalName;
    }
}