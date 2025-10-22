using ChatBot.bot.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using Newtonsoft.Json;
using TwitchAPI.client;
using TwitchAPI.client.data;
using TwitchAPI.helix;

namespace ChatBot.bot.services.moderation.data;

public class ModAction {
    private static readonly ModerationOptions _options = (ModerationOptions)ServiceManager.GetService(ServiceName.Moderation).Options;
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);

    [JsonProperty("name")] 
    public string Name { get; private set; } = null!;

    [JsonProperty("pattern_index")]
    public int PatternIndex { get; private set; }
    
    [JsonProperty("duration")]
    public int Duration { get; private set; }
    
    [JsonProperty("clear_after_stream")]
    public bool ClearAfterStream { get; private set; }
    
    [JsonProperty("max_warn_count")]
    public int MaxWarnCount { get; private set; }
    
    [JsonProperty("moderator_comment")]
    public string ModeratorComment { get; private set; } = null!;
    
    [JsonProperty("type")]
    public ModerationActionType Type { get; private set; }
    
    [JsonProperty("restriction")]
    public Restriction Restriction { get; private set; }
    
    [JsonProperty("is_default")]
    public bool IsDefault { get; private set; }
    
    [JsonProperty("state")]
    public State State { get; private set; }

    
    public ModAction() {}
    
    public ModAction(string name, int patternIndex, int duration, string moderatorComment, Restriction restriction, bool isDefault = false) {
        Name = name;
        PatternIndex = patternIndex;
        Duration = duration;
        ClearAfterStream = false;
        MaxWarnCount = 1;
        ModeratorComment = moderatorComment;
        Type = ModerationActionType.Timeout;
        Restriction = restriction;
        IsDefault = isDefault;
        State = State.Disabled;
    }

    public ModAction(string name, int patternIndex, string moderatorComment, Restriction restriction, bool isDefault = false) {
        Name = name;
        PatternIndex = patternIndex;
        ClearAfterStream = false;
        ModeratorComment = moderatorComment;
        Type = ModerationActionType.Ban;
        Restriction = restriction;
        MaxWarnCount = 1;
        IsDefault = isDefault;
        State = State.Disabled;
    }
    
    public ModAction(string name, int patternIndex, string moderatorComment, ModerationActionType actionType, Restriction restriction, bool isDefault = false) {
        Name = name;
        PatternIndex = patternIndex;
        ClearAfterStream = false;
        ModeratorComment = moderatorComment;
        MaxWarnCount = 1;
        Type = actionType;
        Restriction = restriction;
        IsDefault = isDefault;
        State = State.Disabled;
    }
    
    public ModAction(string name, int patternIndex, string moderatorComment, int maxWarnCount, Restriction restriction, bool isDefault = false, bool clearAfterStream = false) {
        Name = name;
        PatternIndex = patternIndex;
        ClearAfterStream = clearAfterStream;
        MaxWarnCount = maxWarnCount;
        ModeratorComment = moderatorComment;
        Type = ModerationActionType.WarnWithBan;
        Restriction = restriction;
        IsDefault = isDefault;
        State = State.Disabled;
    }
    
    public ModAction(string name, int patternIndex, int duration, string moderatorComment, int maxWarnCount, Restriction restriction, bool isDefault = false, bool clearAfterStream = false) {
        Name = name;
        PatternIndex = patternIndex;
        Duration = duration;
        ClearAfterStream = clearAfterStream;
        MaxWarnCount = maxWarnCount;
        ModeratorComment = moderatorComment;
        Type = ModerationActionType.WarnWithTimeout;
        Restriction = restriction;
        IsDefault = isDefault;
        State = State.Disabled;
    }
    
    [JsonConstructor]
    public ModAction(
        [JsonProperty("name")] string name, 
        [JsonProperty("pattern_index")] int patternIndex,
        [JsonProperty("duration")] int duration,
        [JsonProperty("clear_after_stream")] bool clearAfterStream,
        [JsonProperty("max_warn_count")] int maxWarnCount,
        [JsonProperty("moderator_comment")] string moderatorComment,
        [JsonProperty("type")] ModerationActionType type,
        [JsonProperty("restriction")] Restriction restriction,
        [JsonProperty("is_default")] bool isDefault) {
        Name = name;
        PatternIndex = patternIndex;
        Duration = duration;
        ClearAfterStream = clearAfterStream;
        MaxWarnCount = maxWarnCount;
        ModeratorComment = moderatorComment;
        Type = type;
        Restriction = restriction;
        IsDefault = isDefault;
        State = State.Disabled;
    }

    public async Task Activate(ITwitchClient? client, ChatMessage chatMessage, bool bypass = false) {
        if (State == State.Disabled && !bypass) return;
        if (chatMessage.Fits(Restriction) && !bypass) return;
        if (client?.Credentials == null) return;
        
        switch (Type) {
            case ModerationActionType.Ban: {
                await Helix.BanUser(chatMessage.Username, ModeratorComment, client.Credentials);
                break;
            }
            case ModerationActionType.Timeout: {
                await Helix.TimeoutUser(chatMessage.UserId, ModeratorComment, TimeSpan.FromSeconds(Duration), client.Credentials, (_, message) => {
                                                      _logger.Log(LogLevel.Error, message);
                                                  });
                break;
            }
        }
    }

    public async Task ActivateWarn(ITwitchClient? client, ChatMessage chatMessage, bool bypass = false) {
        if (State == State.Disabled && !bypass) return;
        if (chatMessage.Fits(Restriction) && !bypass) return;
        if (client?.Credentials == null) return;
        
        if (Type == ModerationActionType.Warn) {
            await client.SendMessage(ModeratorComment, chatMessage.Id);
            await Helix.DeleteMessage(chatMessage, client.Credentials, (_, callback) => {
                                                                            _logger.Log(LogLevel.Error, callback);
                                                                        });
            return;
        }
        
        var user = new WarnedUser();
        var userIndex = -1;
        var found = false;

        for (var index = 0; index < _options.WarnedUsers.Count; index++) {
            userIndex = index;
            if (!_options.WarnedUsers[index].UserId.Equals(chatMessage.UserId)) continue;

            found = true;
            user = _options.WarnedUsers[index];
            break;
        }
        
        if (!found) {
            user = new WarnedUser(chatMessage.UserId, this);
            _options.AddWarnedUser(user);
            ++userIndex;
        }
        
        user.GiveWarn();
        await client.SendMessage($"@{chatMessage.Username} -> {ModeratorComment} ({user.Warns}/{MaxWarnCount})");
        if (user.Warns < user.ModAction.MaxWarnCount) {
            await Helix.DeleteMessage(chatMessage, client.Credentials, (_, callback) => {
                                                                            _logger.Log(LogLevel.Error, callback);
                                                                        });
            return;
        }
        
        switch (Type) {
            case ModerationActionType.WarnWithBan: {
                await Helix.BanUser(chatMessage.Username, ModeratorComment, client.Credentials, (_, callback) => {
                                             _logger.Log(LogLevel.Error, callback);
                                         });
                _options.RemoveWarnedUser(userIndex);
                break;
            }
            case ModerationActionType.WarnWithTimeout: {
                await Helix.TimeoutUser(chatMessage.UserId, ModeratorComment, TimeSpan.FromSeconds(Duration), client.Credentials, (_, callback) => {
                                                      _logger.Log(LogLevel.Error, callback);
                                                  });
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

    public bool GetClearAfterStream() {
        return ClearAfterStream;
    }
    
    public void SetClearAfterStream(bool value) {
        ClearAfterStream = value;
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