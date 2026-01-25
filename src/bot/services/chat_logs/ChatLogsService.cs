using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_logs.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.shared;
using TwitchAPI.client.data;

namespace ChatBot.bot.services.chat_logs;

public delegate void LogsAppendedHandler(ChatMessage message);

public class ChatLogsService : Service {
    public override ChatLogsOptions Options { get; } = new ChatLogsOptions();
    public event LogsAppendedHandler? OnLogsAppended;


    public void HandleMessage(ChatMessage chatMessage, FilterStatus status, int patternIndex) {
        if (Options.ServiceState == State.Disabled) return;
        if (status == FilterStatus.Filtered) return;
        
        var msg = new Message(chatMessage.Text, chatMessage.UserId);
        Options.AddLog(msg);
        OnLogsAppended?.Invoke(chatMessage);
    }
    
    public List<Message> GetLogs() {
        return Options.GetLogs();
    }

    public int GetLogsCount() {
        return Options.GetLogsCount();
    }
}