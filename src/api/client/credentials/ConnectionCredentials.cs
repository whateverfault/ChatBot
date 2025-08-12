namespace ChatBot.api.client.credentials;

public class ConnectionCredentials {
    public string Username { get; }
    public string Channel { get; }
    public string OAuth { get; }

    
    public ConnectionCredentials(
        string username,
        string channel,
        string oauth) {
        Username = username;
        Channel = channel;
        OAuth = oauth;
    }
}