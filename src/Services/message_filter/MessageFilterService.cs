using System.Text.RegularExpressions;
using ChatBot.bot.interfaces;
using ChatBot.CLI.CliNodes.Directories;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.Static;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace ChatBot.Services.message_filter;

public enum FilterStatus {
    Match,
    NotMatch
}

public delegate void MessageHandler(ChatMessage message, FilterStatus status, int patternIndex);

public class MessageFilterService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    public override string Name => ServiceName.MessageFilter;
    public override MessageFilterOptions Options { get; } = new();
    public event MessageHandler? OnMessageFiltered;


    public void HandleMessage(object? sender, OnMessageReceivedArgs args) {
        var filters = Options.GetFilters();
        var message = args.ChatMessage.Message;
        var status = FilterStatus.NotMatch;
        var index = 0;

        for (var i = 0; i < filters.Count; i++) {
            var regex = new Regex(filters[i].Pattern);
            if (!regex.Match(message).Success) continue;
            
            status = FilterStatus.Match;
            index = i;
            break;
        }

        OnMessageFiltered?.Invoke(args.ChatMessage, status, index);
    }

    public void AddFilter(Filter filter) {
        Options.AddFilter(filter);
    }

    public List<Filter> GetFilters() {
        return Options.GetFilters();
    }

    public void RemovePattern(int index) {
        var patterns = Options.GetFilters();
        if (index > patterns.Count || index < 0) {
            return;
        }

        Options.RemovePattern(index);
    }

    public override void Init(Bot bot) {
        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }
    }

    public override State GetServiceState() {
        return Options.GetState();
    }
    
    public override void ToggleService() {
        Options.SetState(Options.ServiceState == State.Enabled ? State.Disabled : State.Enabled);
    }
}