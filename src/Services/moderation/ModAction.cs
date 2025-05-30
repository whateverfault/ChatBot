using Newtonsoft.Json;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.moderation;

public class ModAction {
    [JsonProperty(PropertyName = "pattern_index")]
    public int PatternIndex { get; private set; }
    [JsonProperty(PropertyName = "duration")]
    public int Duration { get; private set; }
    [JsonProperty(PropertyName = "max_warn_count")]
    public int MaxWarnCount { get; private set; }
    [JsonProperty(PropertyName = "moderator_comment")]
    public string ModeratorComment { get; private set; }
    [JsonProperty(PropertyName = "type")]
    public ModerationActionType Type { get; private set; }

    
    public ModAction() {}
    
    public ModAction(int patternIndex, int duration, string moderatorComment) {
        Type = ModerationActionType.Timeout;
        PatternIndex = patternIndex;
        Duration = duration;
        ModeratorComment = moderatorComment;
        MaxWarnCount = 1;
    }

    public ModAction(int patternIndex, string moderatorComment) {
        Type = ModerationActionType.Ban;
        PatternIndex = patternIndex;
        ModeratorComment = moderatorComment;
        MaxWarnCount = 1;
    }
    
    public ModAction(int patternIndex, string moderatorComment, ModerationActionType actionType) {
        Type = actionType;
        PatternIndex = patternIndex;
        ModeratorComment = moderatorComment;
        MaxWarnCount = 1;
    }
    
    public ModAction(int patternIndex, string moderatorComment, int maxWarnCount) {
        Type = ModerationActionType.WarnWithBan;
        PatternIndex = patternIndex;
        ModeratorComment = moderatorComment;
        MaxWarnCount = maxWarnCount;
    }
    
    public ModAction(int patternIndex, int duration, string moderatorComment, int maxWarnCount) {
        Type = ModerationActionType.WarnWithTimeout;
        PatternIndex = patternIndex;
        Duration = duration;
        ModeratorComment = moderatorComment;
        MaxWarnCount = maxWarnCount;
    }
    
    [JsonConstructor]
    public ModAction(
        [JsonProperty("pattern_index")] int patternIndex,
        [JsonProperty("duration")] int duration,
        [JsonProperty("max_warn_count")] int maxWarnCount,
        [JsonProperty("moderator_comment")] string moderatorComment,
        [JsonProperty("type")] ModerationActionType type
        ) {
        PatternIndex = patternIndex;
        Duration = duration;
        MaxWarnCount = maxWarnCount;
        ModeratorComment = moderatorComment;
        Type = type;
    }

    public void Activate(ITwitchClient client, ChatMessage message) {
        switch (Type) {
            case ModerationActionType.Ban: {
                client.BanUser(message.Channel, message.Username, ModeratorComment);
                break;
            }
            case ModerationActionType.Timeout: {
                client.TimeoutUser(message.Channel, message.Username, TimeSpan.FromSeconds(Duration), ModeratorComment);
                break;
            }
        }
    }

    public void ActivateWarn(ITwitchClient client, ChatMessage message, List<WarnedUser> warnedUsers) {
        if (Type == ModerationActionType.Warn) {
            client.SendReply(message.Channel, message.Id, ModeratorComment);
            return;
        }
        
        var user = new WarnedUser();
        var userIndex = -1;
        var found = false;

        for (var index = 0; index < warnedUsers.Count; index++) {
            if (!warnedUsers[index].UserId.Equals(message.UserId)) continue;

            found = true;
            userIndex = index;
            user = warnedUsers[index];
        }
        if (!found) {
            warnedUsers.Add(new WarnedUser(message.UserId, this));
            return;
        }
        
        user.GiveWarn();
        if (user.Warns != user.ModAction.MaxWarnCount) return;
        
        switch (Type) {
            case ModerationActionType.WarnWithBan: {
                client.BanUser(message.Channel, message.Username, ModeratorComment);
                warnedUsers.RemoveAt(userIndex);
                break;
            }
            case ModerationActionType.WarnWithTimeout: {
                client.TimeoutUser(message.Channel, message.Username, TimeSpan.FromSeconds(Duration), ModeratorComment);
                warnedUsers.RemoveAt(userIndex);
                break;
            }
        }
    }
    
    public int GetIndex() {
        return PatternIndex;
    }

    public void SetIndex(int index) {
        PatternIndex = index;
    }
    
    public int GetDuration() {
        return Duration;
    }

    public void SetDuration(int index) {
        Duration = index;
    }

    public string GetComment() {
        return ModeratorComment;
    }

    public void SetComment(string comment) {
        ModeratorComment = comment;
    }
    
    public int GetMaxWarnCount() {
        return MaxWarnCount;
    }

    public void SetMaxWarnCount(int count) {
        MaxWarnCount = count;
    }
}