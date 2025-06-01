using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.game_requests;

public class SaveData {
    [JsonProperty(PropertyName ="game_requests")]
    public List<GameRequest>? GameRequests { get; }
    [JsonProperty(PropertyName ="game_requests_points")]
    public Dictionary<string, int>? GameRequestsPoints { get; }
    [JsonProperty(PropertyName ="game_requests_hash_set")]
    public HashSet<int>? GameRequestsSet { get; }
    [JsonProperty(PropertyName ="service_state")]
    public State ServiceState { get; set; }


    public SaveData(State serviceState, List<GameRequest>? gameRequests, HashSet<int>? gameRequestsSet, Dictionary<string, int>? gameRequestsPoints) {
        ServiceState = serviceState;
        GameRequests = gameRequests;
        GameRequestsSet = gameRequestsSet;
        GameRequestsPoints = gameRequestsPoints;
    }
}