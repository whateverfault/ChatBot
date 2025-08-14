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


    public void HandleMessage(object? sender, ChatMessage chatMessage) {
        var filters = Options.GetFilters();
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
    }

    public List<Filter> GetFilters() {
        return Options.GetFilters();
    }

    public bool RemovePattern(int index) {
        var patterns = Options.GetFilters();
        if (index > patterns.Count || index < 0) {
            return false;
        }

        Options.RemovePattern(index);
        return true;
    }

    public override State GetServiceState() {
        return Options.GetState();
    }
}