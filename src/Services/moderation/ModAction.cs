using ChatBot.bot;
using ChatBot.services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils.Twitch.Helix;
using Newtonsoft.Json;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.services.moderation;

public class ModAction {
    private static readonly ModerationOptions _options = (ModerationOptions)ServiceManager.GetService(ServiceName.Moderation).Options;

    [JsonProperty("name")] 
    public string Name { get; private set; } = null!;

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
    
    [JsonProperty(PropertyName = "is_default")]
    public bool IsDefault { get; private set; }
    
    [JsonProperty(PropertyName = "state")]
    public State State { get; private set; }

    
    public ModAction() {}
    
    public ModAction(string name, int patternIndex, int duration, string moderatorComment, Restriction restriction, bool isDefault = false) {
        Name = name;
        Type = ModerationActionType.Timeout;
        PatternIndex = patternIndex;
        Duration = duration;
        ModeratorComment = moderatorComment;
        Restriction = restriction;
        MaxWarnCount = 1;
        IsDefault = isDefault;
        State = State.Disabled;
    }

    public ModAction(string name, int patternIndex, string moderatorComment, Restriction restriction, bool isDefault = false) {
        Name = name;
        Type = ModerationActionType.Ban;
        PatternIndex = patternIndex;
        ModeratorComment = moderatorComment;
        Restriction = restriction;
        MaxWarnCount = 1;
        IsDefault = isDefault;
        State = State.Disabled;
    }
    
    public ModAction(string name, int patternIndex, string moderatorComment, ModerationActionType actionType, Restriction restriction, bool isDefault = false) {
        Name = name;
        Type = actionType;
        PatternIndex = patternIndex;
        ModeratorComment = moderatorComment;
        Restriction = restriction;
        MaxWarnCount = 1;
        IsDefault = isDefault;
        State = State.Disabled;
    }
    
    public ModAction(string name, int patternIndex, string moderatorComment, int maxWarnCount, Restriction restriction, bool isDefault = false) {
        Name = name;
        Type = ModerationActionType.WarnWithBan;
        PatternIndex = patternIndex;
        ModeratorComment = moderatorComment;
        MaxWarnCount = maxWarnCount;
        Restriction = restriction;
        IsDefault = isDefault;
        State = State.Disabled;
    }
    
    public ModAction(string name, int patternIndex, int duration, string moderatorComment, int maxWarnCount, Restriction restriction, bool isDefault = false) {
        Name = name;
        Type = ModerationActionType.WarnWithTimeout;
        PatternIndex = patternIndex;
        Duration = duration;
        ModeratorComment = moderatorComment;
        MaxWarnCount = maxWarnCount;
        Restriction = restriction;
        IsDefault = isDefault;
        State = State.Disabled;
    }
    
    [JsonConstructor]
    public ModAction(
        [JsonProperty("name")] string name, 
        [JsonProperty("pattern_index")] int patternIndex,
        [JsonProperty("duration")] int duration,
        [JsonProperty("max_warn_count")] int maxWarnCount,
        [JsonProperty("moderator_comment")] string moderatorComment,
        [JsonProperty("type")] ModerationActionType type,
        [JsonProperty("restriction")] Restriction restriction,
        [JsonProperty("is_default")] bool isDefault) {
        Name = name;
        PatternIndex = patternIndex;
        Duration = duration;
        MaxWarnCount = maxWarnCount;
        ModeratorComment = moderatorComment;
        Type = type;
        Restriction = restriction;
        IsDefault = isDefault;
        State = State.Disabled;
    }

    public async Task Activate(ITwitchClient? client, ChatBotOptions botOptions, ChatMessage message, bool bypass = false) {
        if (State == State.Disabled && !bypass) return;
        if (RestrictionHandler.Handle(Restriction, message) && !bypass) return;
        
        switch (Type) {
            case ModerationActionType.Ban: {
                await HelixUtils.BanUser(botOptions, message.Username, ModeratorComment);
                break;
            }
            case ModerationActionType.Timeout: {
                client.TimeoutUser(message.Channel, message.Username, TimeSpan.FromSeconds(Duration), ModeratorComment);
                break;
            }
        }
    }

    public async Task ActivateWarn(ITwitchClient? client, ChatBotOptions options, ChatMessage message, bool bypass = false) {
        if (State == State.Disabled && !bypass) return;
        if (RestrictionHandler.Handle(Restriction, message) && !bypass) return;
        
        if (Type == ModerationActionType.Warn) {
            client?.SendReply(message.Channel, message.Id, ModeratorComment);
            await HelixUtils.DeleteMessage(options, message);
            return;
        }
        
        var user = new WarnedUser();
        var userIndex = -1;
        var found = false;

        for (var index = 0; index < _options.WarnedUsers.Count; index++) {
            userIndex = index;
            if (!_options.WarnedUsers[index].UserId.Equals(message.UserId)) continue;

            found = true;
            user = _options.WarnedUsers[index];
            break;
        }
        
        if (!found) {
            user = new WarnedUser(message.UserId, this);
            _options.AddWarnedUser(user);
            ++userIndex;
        }
        
        user.GiveWarn();
        client?.SendMessage(message.Channel, $"@{message.Username} -> {ModeratorComment} ({user.Warns}/{MaxWarnCount})");
        if (user.Warns < user.ModAction.MaxWarnCount) {
            await HelixUtils.DeleteMessage(options, message);
            return;
        }
        
        switch (Type) {
            case ModerationActionType.WarnWithBan: {
                await HelixUtils.BanUser(options, message.Username, ModeratorComment);
                _options.RemoveWarnedUser(userIndex);
                break;
            }
            case ModerationActionType.WarnWithTimeout: {
                await HelixUtils.TimeoutUserHelix(options, message.Username, TimeSpan.FromSeconds(Duration), ModeratorComment);
                _options.RemoveWarnedUser(userIndex);
                break;
            }
        }
    }

    public string GetName() {
        return Name;
    }

    public void SetName(string name) {
        Name = name;
        _options.Save();
    }
    
    public int GetIndex() {
        return PatternIndex;
    }

    public bool SetIndex(int index) {
        if (index < 0 || index >= _options.ModerationActions.Count) {
            return false;
        }
        
        PatternIndex = index;
        _options.Save();
        return true;
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