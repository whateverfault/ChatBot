using ChatBot.Services.interfaces;
using ChatBot.Shared.interfaces;

namespace ChatBot.Services.game_requests;

public class SaveData {
    public List<GameRequest>? gameRequests = [];
    public Dictionary<string, int>? gameRequestsPoints = [];
    public State state;
}