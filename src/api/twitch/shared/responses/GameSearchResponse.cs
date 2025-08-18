using ChatBot.api.twitch.shared.requests.data;

namespace ChatBot.api.twitch.shared.responses;

public class GameSearchResponse {
    public List<GameData>? Data { get; set; }
}