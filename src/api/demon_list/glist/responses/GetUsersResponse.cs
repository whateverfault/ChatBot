using ChatBot.api.demon_list.glist.data;
using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.responses;

public class GetUsersResponse {
    [JsonProperty("data")]
    public UsersData Data { get; set; }
    
    
    public GetUsersResponse([JsonProperty("data")] UsersData data) {
        Data = data;
    }
}