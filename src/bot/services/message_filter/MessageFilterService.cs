using System.Text.RegularExpressions;
using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter.data;
using ChatBot.bot.shared.handlers;
using TwitchAPI.api.data.responses.GetUserInfo;
using TwitchAPI.client;
using TwitchAPI.client.commands.data;
using TwitchAPI.client.data;

namespace ChatBot.bot.services.message_filter;

public enum FilterStatus {
    Filtered,
    Ok,
}

public delegate void CommandHandler(Command parsedCommand, FilterStatus status, int filterIndex);
public delegate void MessageHandler(ChatMessage message, FilterStatus status, int filterIndex);

public struct FilterResult {
    public FilterStatus Status;
    public int PatternIndex;


    public FilterResult(FilterStatus status, int patternIndex) {
        Status = status;
        PatternIndex = patternIndex;
    }
}

public class MessageFilterService : Service {
    public override MessageFilterOptions Options { get; } = new MessageFilterOptions();
    public event CommandHandler? OnCommandFiltered;
    public event MessageHandler? OnMessageFiltered;

    public event EventHandler<Filter>? OnFilterAdded;
    public event EventHandler<int>? OnFilterRemoved;
    
    public event EventHandler<UserInfo>? OnBannedUserAdded;
    public event EventHandler<int>? OnBannedUserRemoved;
    
    
    public void HandleCommand(object? sender, Command cmd) {
        var result = FilterMessage(cmd.ChatMessage, skipFilters: x => x.IsDefault);

        OnCommandFiltered?.Invoke(cmd, result.Status, result.PatternIndex);
    }
    
    public void HandleMessage(object? sender, ChatMessage chatMessage) {
        var result = FilterMessage(chatMessage);

        OnMessageFiltered?.Invoke(chatMessage, result.Status, result.PatternIndex);
    }

    private FilterResult FilterMessage(ChatMessage message, Func<Filter, bool>? skipFilters = null) {
        var result = new FilterResult(FilterStatus.Ok, -1);
        var filters = GetFilters();
        var bannedUsers = GetBannedUsers();

        if (bannedUsers.Any(x => x.UserId.Equals(message.UserId)))
            result.Status = FilterStatus.Filtered;
        
        for (var i = 0; i < filters.Count; i++) {
            if (result.Status == FilterStatus.Filtered)
                break;

            var filter = filters[i];
            
            if (skipFilters != null && skipFilters(filter))
                continue;
            
            var regex = new Regex(filter.Pattern);
            if (!regex.Match(message.Text).Success) continue;
            
            result.Status = FilterStatus.Filtered;
            result.PatternIndex = i;
        }

        return result;
    }
    
    public void AddFilter(Filter filter) {
        Options.AddFilter(filter);
        OnFilterAdded?.Invoke(this, filter);
    }

    public bool RemoveFilter(int index) {
        var result = Options.RemoveFilter(index);
        if (result) 
            OnFilterRemoved?.Invoke(this, index);
        
        return result;
    }

    public IReadOnlyList<Filter> GetFilters() {
        return Options.GetFilters();
    }

    public async Task AddBannedUser(string username) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client?.Credentials == null)
            return;
        
        var api = TwitchChatBot.Instance.Api;

        var userInfo = await api.GetUserInfoByUserName(username, client.Credentials, (_, message) => {
                                                               ErrorHandler.LogMessage(LogLevel.Error, message);
                                                           });
        if (userInfo == null)
            return;

        Options.AddBannedUser(userInfo);
        OnBannedUserAdded?.Invoke(this, userInfo);
    }

    public void AddBannedUser(UserInfo userInfo) {
        Options.AddBannedUser(userInfo);
        OnBannedUserAdded?.Invoke(this, userInfo);
    }
    
    public bool RemoveBannedUser(int index) {
        var result = Options.RemoveBannedUser(index);
        if (result) 
            OnBannedUserRemoved?.Invoke(this, index);
        
        return result;
    }
    
    public bool RemoveBannedUser(UserInfo userInfo) {
        var index = Options.RemoveBannedUser(userInfo);
        if (index >= 0) 
            OnBannedUserRemoved?.Invoke(this, index);
        
        return index >= 0;
    }
    
    public IReadOnlyList<UserInfo> GetBannedUsers() {
        return Options.GetBannedUsers();
    }
    
    public override State GetServiceState() {
        return Options.GetState();
    }

    public void SubscribeToBannedUserAdded(EventHandler<UserInfo> subscriber) {
        OnBannedUserAdded += subscriber;
    }
    
    public void SubscribeToBannedUserRemoved(EventHandler<int> subscriber) {
        OnBannedUserRemoved += subscriber;
    }
}