using ChatBot.shared.interfaces;

namespace ChatBot.Services.game_requests;

public class SaveData {
    public State state;
    public readonly List<GameRequest>? gameRequests;
    public readonly HashSet<int>? gameRequestsSet;
    public readonly Dictionary<string, int>? gameRequestsPoints;


    public SaveData(State state, List<GameRequest>? gameRequests, HashSet<int>? gameRequestsSet, Dictionary<string, int>? gameRequestsPoints) {
        this.state = state;
        this.gameRequests = gameRequests;
        this.gameRequestsSet = gameRequestsSet;
        this.gameRequestsPoints = gameRequestsPoints;
    }
}