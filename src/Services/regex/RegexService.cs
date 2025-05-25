using System.Text.RegularExpressions;
using ChatBot.Services.interfaces;
using ChatBot.Services.Static;
using ChatBot.shared.interfaces;
using ChatBot.shared.Logging;
using ChatBot.twitchAPI.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace ChatBot.Services.regex;

public enum FilterStatus {
    Match,
    NotMatch
}

public delegate void HandleMessage(ChatMessage message, FilterStatus status);

public class RegexService : Service {
    public override string Name => ServiceName.Regex;
    public override RegexOptions Options { get; } = new();
    public event HandleMessage OnMessageFiltered;


    public void HandleMessage(object? sender, OnMessageReceivedArgs args) {
        var patterns = Options.GetPatterns();
        var message = args.ChatMessage.Message;
        var status = FilterStatus.NotMatch;

        foreach (var pattern in patterns) {
            if (pattern.Match(message).Success) {
                status = FilterStatus.Match;
            }
        }

        OnMessageFiltered(args.ChatMessage, status);
    }

    public void AddPattern(string pattern) {
        Options.AddPattern(new Regex(pattern));
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

    public override dynamic GetServiceStateDynamic() {
        return Options.State;
    }
    
    public override void ToggleService() {
        Options.SetState(Options.State == State.Enabled ? State.Disabled : State.Enabled);
    }
}