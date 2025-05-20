using ChatBot.Shared.interfaces;

namespace ChatBot.twitchAPI;

public class SaveData {
    public State state; 
    public string? username;
    public string? oAuth;
    public string? channel;
    public bool shouldPrintTwitchLogs;
}