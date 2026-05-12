using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.data;

public class UsersData {
    [JsonProperty("users")]
    public List<UserData> Users { get; set; }


    public UsersData([JsonProperty("users")] List<UserData> users) {
        Users = users;
    }
}