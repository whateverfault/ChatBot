using ChatBot.bot;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils.Helix;
using Newtonsoft.Json;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.moderation;

public class ModAction {
    private static readonly Options _options = ServiceManager.GetService(ServiceName.Moderation).Options;

    [JsonProperty(PropertyName = "pattern_index")]
    public int PatternIndex { get; private set; }
    [JsonProperty(PropertyName = "duration")]
    public int Duration { get; private set; }
    [JsonProperty(PropertyName = "max_warn_count")]
    public int MaxWarnCount { get; private set; }
    [JsonProperty(PropertyName = "moderator_comment")]
    public string ModeratorComment { get; private set; } = null!;

    [JsonProperty(PropertyName = "type")]
    public ModerationActionType Type { get; private set; }
    [JsonProperty(PropertyName = "restriction")]
    public Restriction Restriction { get; private set; }
    [JsonProperty(PropertyName = "state")]
    public State State { get; private set; }

    
    public ModAction() {}
    
    public ModAction(int patternIndex, int duration, string moderatorComment, Restriction restriction) {
        Type = ModerationActionType.Timeout;
        PatternIndex = patternIndex;
        Duration = duration;
        ModeratorComment = moderatorComment;
        Restriction = restriction;
        State = State.Disabled;
        MaxWarnCount = 1;
    }

    public ModAction(int patternIndex, string moderatorComment, Restriction restriction) {
        Type = ModerationActionType.Ban;
        PatternIndex = patternIndex;
        ModeratorComment = moderatorComment;
        Restriction = restriction;
        State = State.Disabled;
        MaxWarnCount = 1;
    }
    
    public ModAction(int patternIndex, string moderatorComment, ModerationActionType actionType, Restriction restriction) {
        Type = actionType;
        PatternIndex = patternIndex;
        ModeratorComment = moderatorComment;
        Restriction = restriction;
        State = State.Disabled;
        MaxWarnCount = 1;
    }
    
    public ModAction(int patternIndex, string moderatorComment, int maxWarnCount, Restriction restriction) {
        Type = ModerationActionType.WarnWithBan;
        PatternIndex = patternIndex;
        ModeratorComment = moderatorComment;
        MaxWarnCount = maxWarnCount;
        Restriction = restriction;
        State = State.Disabled;
    }
    
    public ModAction(int patternIndex, int duration, string moderatorComment, int maxWarnCount, Restriction restriction) {
        Type = ModerationActionType.WarnWithTimeout;
        PatternIndex = patternIndex;
        Duration = duration;
        ModeratorComment = moderatorComment;
        MaxWarnCount = maxWarnCount;
        Restriction = restriction;
        State = State.Disabled;
    }
    
    [JsonConstructor]
    public ModAction(
        [JsonProperty("pattern_index")] int patternIndex,
        [JsonProperty("duration")] int duration,
        [JsonProperty("max_warn_count")] int maxWarnCount,
        [JsonProperty("moderator_comment")] string moderatorComment,
        [JsonProperty("type")] ModerationActionType type,
        [JsonProperty("restriction")] Restriction restriction
        ) {
        PatternIndex = patternIndex;
        Duration = duration;
        MaxWarnCount = maxWarnCount;
        ModeratorComment = moderatorComment;
        Type = type;
        Restriction = restriction;
        State = State.Disabled;
    }

    public async Task Activate(ITwitchClient? client, ChatBotOptions botOptions, ChatMessage message, bool bypass = false) {
        if (State == State.Disabled && !bypass) return;
        if (RestrictionHandler.Handle(Restriction, message) && !bypass) return;
        
        switch (Type) {
            case ModerationActionType.Ban: {
                await HelixUtils.BanUserHelix(botOptions, message.Username, ModeratorComment);
                break;
            }
            case ModerationActionType.Timeout: {
                client.TimeoutUser(message.Channel, message.Username, TimeSpan.FromSeconds(Duration), ModeratorComment);
                break;
            }
        }
    }

    public async Task ActivateWarn(ITwitchClient? client, ChatBotOptions options, ChatMessage message, List<WarnedUser> warnedUsers, bool bypass = false) {
        if (State == State.Disabled && !bypass) return;
        if (RestrictionHandler.Handle(Restriction, message) && !bypass) return;
        
        if (Type == ModerationActionType.Warn) {
            client?.SendReply(message.Channel, message.Id, ModeratorComment);
            await HelixUtils.DeleteMessageHelix(options, message);
            return;
        }
        
        var user = new WarnedUser();
        var userIndex = -1;
        var found = false;

        for (var index = 0; index < warnedUsers.Count; index++) {
            userIndex = index;
            if (!warnedUsers[index].UserId.Equals(message.UserId)) continue;

            found = true;
            user = warnedUsers[index];
        }
        if (!found) {
            user = new WarnedUser(message.UserId, this);
            warnedUsers.Add(user);
            userIndex++;
        }
        
        user.GiveWarn();
        client?.SendMessage(message.Channel, $"@{message.Username} {ModeratorComment} ({user.Warns}/{MaxWarnCount})");
        if (user.Warns < user.ModAction.MaxWarnCount) {
            await HelixUtils.DeleteMessageHelix(options, message);
            return;
        }
        
        switch (Type) {
            case ModerationActionType.WarnWithBan: {
                await HelixUtils.BanUserHelix(options, message.Username, ModeratorComment);
                warnedUsers.RemoveAt(userIndex);
                break;
            }
            case ModerationActionType.WarnWithTimeout: {
                await HelixUtils.TimeoutUserHelix(options, message.Username, TimeSpan.FromSeconds(Duration), ModeratorComment);
                warnedUsers.RemoveAt(userIndex);
                break;
            }
        }
        
        _options.Save();
    }
    
    public int GetIndex() {
        return PatternIndex;
    }

    public void SetIndex(int index) {
        PatternIndex = index;
        _options.Save();
    }
    
    public int GetDuration() {
        return Duration;
    }

    public void SetDuration(int index) {
        Duration = index;
        _options.Save();
    }

    public string GetComment() {
        return ModeratorComment;
    }

    public void SetComment(string comment) {
        ModeratorComment = comment;
        _options.Save();
    }
    
    public int GetMaxWarnCount() {
        return MaxWarnCount;
    }

    public void SetMaxWarnCount(int count) {
        MaxWarnCount = count;
        _options.Save();
    }

    public int GetRestrictionAsInt() {
        return (int)Restriction;
    }

    public void RestrictionNext() {
        Restriction = (Restriction)(((int)Restriction+1)%Enum.GetValues(typeof(Restriction)).Length);
        _options.Save();
    }
    
    public int GetStateAsInt() {
        return (int)State;
    }

    public void StateNext() {
        State = (State)(((int)State+1)%Enum.GetValues(typeof(State)).Length);
        _options.Save();
    }
}