using Newtonsoft.Json;

namespace ChatBot.api.client.credentials;

public class FullCredentials {
    [JsonProperty("username")]
    public string Username { get; private set; }

    [JsonProperty("channel")]
    public string Channel { get; private set; }

    [JsonProperty("oauth")]
    public string OAuth { get; private set; }
    
    [JsonProperty("channel_oauth")]
    public string ChannelOAuth { get; private set; }
    
    [JsonProperty("client_id")]
    public string ClientId { get; private set; }
    
    [JsonProperty("user_id")]
    public string UserId { get; private set; }

    [JsonProperty("channel_id")]
    public string ChannelId { get; private set; }


    public FullCredentials() {
        Username = string.Empty;
        Channel = string.Empty;
        OAuth = string.Empty;
        ClientId = string.Empty;
        UserId = string.Empty;
        ChannelId = string.Empty;
    }
    
    public FullCredentials(
        [JsonProperty("username")] string username,
        [JsonProperty("channel")] string channel,
        [JsonProperty("oauth")] string oAuth,
        [JsonProperty("client_id")] string clientId,
        [JsonProperty("user_id")] string userId,
        [JsonProperty("channel_id")] string channelId) {
        Username = username;
        Channel = channel;
        OAuth = oAuth;
        ClientId = clientId;
        UserId = userId;
        ChannelId = channelId;
    }

    public void UpdateUsername(string username) {
        Username = username;
    }
    
    public void UpdateChannel(string channel) {
        Channel = channel;
    }
    
    public void UpdateOauth(string oauth) {
        OAuth = oauth;
    }
    
    public void UpdateClientId(string clientId) {
        ClientId = clientId;
    }
    
    public void UpdateUserId(string userId) {
        userId = userId;
    }
    
    public void UpdateChannelId(string channelId) {
        ChannelId = channelId;
    }
}