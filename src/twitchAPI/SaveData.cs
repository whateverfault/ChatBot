using ChatBot.shared.interfaces;

namespace ChatBot.twitchAPI;

public class SaveData {
    public State state;
    public string? username;
    public string? channel;
    public string? oAuth;
    public bool shouldPrintTwitchLogs;


    public SaveData() {}

    public SaveData(State state, string username, string oAuth, string channel, bool shouldPrintTwitchLogs) {
        this.state = state;
        this.username = username;
        this.oAuth = oAuth;
        this.channel = channel;
        this.shouldPrintTwitchLogs = shouldPrintTwitchLogs;
    }
}