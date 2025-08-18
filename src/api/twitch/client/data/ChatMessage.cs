using System.Text;
using ChatBot.api.twitch.event_sub.subscription_data.events.chat_message;

namespace ChatBot.api.twitch.client.data;

public class ChatMessage {
    public string ChannelId { get; }

    public string Channel { get; }

    public string UserId { get; }

    public string Username { get; }

    public string Id { get; }

    public string Text { get; }
    
    public ChatReply? Reply { get; }
    
    public string? RewardId { get; }
    
    public bool IsBroadcaster { get; }
    
    public bool IsModerator { get; }
    
    public bool IsVip { get; }
    
    public bool IsSubscriber { get; }
    
    
    private ChatMessage(
        string channelId,
        string channel,
        string userId,
        string username,
        string id,
        string text,
        ChatReply? reply,
        string? rewardId, 
        bool isBroadcaster,
        bool isModerator,
        bool isVip,
        bool isSubscriber) {
        ChannelId = channelId;
        Channel = channel;
        UserId = userId;
        Username = username;
        Id = id;
        Text = text;
        Reply = reply;
        RewardId = rewardId;
        IsBroadcaster = isBroadcaster;
        IsModerator = isModerator;
        IsVip = isVip;
        IsSubscriber = isSubscriber;
    }

    public static ChatMessage Create(ChatMessageEvent e) {
        var message = new StringBuilder();

        foreach (var fragment in e.Message.Fragments) {
            if (fragment.Type == "mention") continue;

            var processed = fragment.Text
                                    .Replace($"{(char)56128}", "")
                                    .Replace($"{(char)56320}", "")
                                    .Trim();
            message.Append($"{processed} ");
        }
        
        ParseBadges(e.Badges, out var isBroadcaster, out var isModerator, out var isVip, out var isSubscriber);
        
        var args = new ChatMessage(
                                   e.ChannelId,
                                   e.Channel,
                                   e.UserId,
                                   e.User, 
                                   e.MessageId,
                                   message.ToString(),
                                   e.Reply,
                                   e.RewardId,
                                   isBroadcaster,
                                   isModerator,
                                   isVip,
                                   isSubscriber
                                   );
        return args;
    }

    private static void ParseBadges(BadgeInfo[] badges, out bool isBroadcaster, out bool isModerator, out bool isVip, out bool isSubscriber) {
        isBroadcaster = false;
        isModerator = false;
        isVip = false;
        isSubscriber = false;

        foreach (var badge in badges) {
            switch (badge.Name) {
                case "broadcaster": {
                    isBroadcaster = true;
                    break;
                }
                case "moderator": {
                    isModerator = true;
                    break;
                }
                case "vip": {
                    isVip = true;
                    break;
                }
                case "subscriber": {
                    isSubscriber = true;
                    break;
                }
            }
        }
    }
}