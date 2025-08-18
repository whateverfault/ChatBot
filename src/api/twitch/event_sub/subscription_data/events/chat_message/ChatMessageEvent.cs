using Newtonsoft.Json;

namespace ChatBot.api.twitch.event_sub.subscription_data.events.chat_message;

public class ChatMessageEvent {
    [JsonProperty("broadcaster_user_id")]
    public string ChannelId { get; set; }
    
    [JsonProperty("broadcaster_user_name")]
    public string Channel { get; set; }

    [JsonProperty("chatter_user_id")]
    public string UserId { get; set; }
    
    [JsonProperty("chatter_user_name")]
    public string User { get; set; }
    
    [JsonProperty("message_id")]
    public string MessageId { get; set; }
    
    [JsonProperty("reply")]
    public ChatReply? Reply { get; set; }
    
    [JsonProperty("channel_points_custom_reward_id")]
    public string? RewardId { get; set; }
    
    [JsonProperty("message")]
    public ChatMessage Message { get; set; }
    
    [JsonProperty("badges")]
    public BadgeInfo[] Badges { get; set; }
    
    
    public ChatMessageEvent(
        [JsonProperty("broadcaster_user_id")] string channelId,
        [JsonProperty("broadcaster_user_name")] string channel,
        [JsonProperty("chatter_user_id")] string userId,
        [JsonProperty("chatter_user_name")] string user,
        [JsonProperty("message_id")] string messageId,
        [JsonProperty("reply")] ChatReply? reply,
        [JsonProperty("channel_points_custom_reward_id")] string? rewardId,
        [JsonProperty("message")] ChatMessage message,
        [JsonProperty("badges")] BadgeInfo[] badges) {
        ChannelId = channelId;
        Channel = channel;
        UserId = userId;
        User = user;
        MessageId = messageId;
        Reply = reply;
        RewardId = rewardId;
        Message = message;
        Badges = badges;
    }
}