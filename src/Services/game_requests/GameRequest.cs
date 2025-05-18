namespace ChatBot.Services.game_requests;

public class GameRequest {
    public string Requester { get; }
    public string GameName { get; }


    public GameRequest(string requester, string gameName) {
        Requester = requester;
        GameName = gameName;
    }
}