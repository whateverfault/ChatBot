using System.Text.RegularExpressions;
using ChatBot.Services.interfaces;
using ChatBot.Services.Static;
using ChatBot.shared.interfaces;
using ChatBot.shared.Logging;
using ChatBot.twitchAPI.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace ChatBot.Services.message_filter;

public enum FilterStatus {
    Match,
    NotMatch
}

public delegate void HandleMessage(ChatMessage message, FilterStatus status);

public class MessageFilterService : Service {
    public override string Name => ServiceName.MessageFilter;
    public override MessageFilterOptions Options { get; } = new();
    public event HandleMessage? OnMessageFiltered;


    public void HandleMessage(object? sender, OnMessageReceivedArgs args) {
        var patterns = Options.GetPatterns();
        var message = args.ChatMessage.Message;
        var status = FilterStatus.NotMatch;

        foreach (var pattern in patterns) {
            if (pattern.Match(message).Success) {
                status = FilterStatus.Match;
            }
        }
        
        OnMessageFiltered?.Invoke(args.ChatMessage, status);
    }

    public void AddPattern(string pattern) {
        Options.AddPattern(new Regex(pattern));
    }

    public List<string> GetPatterns() {
        return Options.GetPatterns().Select(pattern => pattern.ToString()).ToList();
    }
    
    public string GetPattern(int index) {
        return Options.GetPatterns()[index].ToString();
    }

    public void RemovePattern(int index) {
        var patterns = Options.GetPatterns();
        if (index > patterns.Count || index < 0) {
            Logger.Log(LogLevel.Error, "Tried to delete not existing pattern");
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
        Options.SetState(Options.State == State.Enabled ? State.Disabled : State.Enabled);
    }
}