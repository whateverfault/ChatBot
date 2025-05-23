using ChatBot.shared.interfaces;

namespace ChatBot.Services.game_requests;

public class SaveData {
    public readonly List<GameRequest>? gameRequests;
    public readonly Dictionary<string, int>? gameRequestsPoints;
    public readonly HashSet<int>? gameRequestsSet;
    public State state;


    public SaveData(State state, List<GameRequest>? gameRequests, HashSet<int>? gameRequestsSet, Dictionary<string, int>? gameRequestsPoints) {
        this.state = state;
        this.gameRequests = gameRequests;
        this.gameRequestsSet = gameRequestsSet;
        this.gameRequestsPoints = gameRequestsPoints;
    }
}