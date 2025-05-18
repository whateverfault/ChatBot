namespace ChatBot.twitchAPI;

public class Options {
    public string? Username { get; }
    public string? OAuth { get; }
    public string? Channel { get; }
    public string? ClientId { get; }
    public string? Secret { get; }
    public bool ShouldCreateReward { get; }
    public bool IsRewardCreated { get; set; }
    
    
    public Options(string? username, string? oAuth, string? channel, bool shouldCreateReward, string? clientId = "", string? secret = "") {
        Username = username;
        OAuth = oAuth;
        Channel = channel;
        ClientId = clientId;
        Secret = secret;
        ShouldCreateReward = shouldCreateReward;
    }
}