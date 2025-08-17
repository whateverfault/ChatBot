using System.Text.RegularExpressions;
using ChatBot.api.client.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.interfaces;

namespace ChatBot.bot.services.message_filter;

public enum FilterStatus {
    Match,
    NotMatch,
}

public delegate void MessageHandler(ChatMessage message, FilterStatus status, int patternIndex);

public class MessageFilterService : Service {
    public override string Name => ServiceName.MessageFilter;
    public override MessageFilterOptions Options { get; } = new MessageFilterOptions();
    public event MessageHandler? OnMessageFiltered;

    public EventHandler<Filter>? OnFilterAdded;
    public EventHandler<int>? OnFilterRemoved;
    

    public void HandleMessage(object? sender, ChatMessage chatMessage) {
        var filters = GetFilters();
        var status = FilterStatus.NotMatch;
        var index = 0;

        for (var i = 0; i < filters.Count; i++) {
            var regex = new Regex(filters[i].Pattern);
            if (!regex.Match(chatMessage.Text).Success) continue;
            
            status = FilterStatus.Match;
            index = i;
            break;
        }

        OnMessageFiltered?.Invoke(chatMessage, status, index);
    }

    public void AddFilter(Filter filter) {
        Options.AddFilter(filter);
        OnFilterAdded?.Invoke(this, filter);
    }

    public bool RemoveFilter(int index) {
        var result = Options.RemoveFilter(index);
        if (result) OnFilterRemoved?.Invoke(this, index);
        
        return result;
    }

    public List<Filter> GetFilters() {
        return Options.GetFilters();
    }
    
    public override State GetServiceState() {
        return Options.GetState();
    }
}