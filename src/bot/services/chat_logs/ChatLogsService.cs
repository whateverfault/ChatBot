using ChatBot.api.client.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;
using ChatBot.shared;
using ChatBot.shared.interfaces;

namespace ChatBot.bot.services.chat_logs;

public delegate void LogsAppendedHandler(ChatMessage message);

public class ChatLogsService : Service {
    public override string Name => ServiceName.ChatLogs;
    public override ChatLogsOptions Options { get; } = new ChatLogsOptions();
    public event LogsAppendedHandler? OnLogsAppended;


    public void HandleMessage(ChatMessage chatMessage, FilterStatus status, int patternIndex) {
        if (Options.ServiceState == State.Disabled) return;
        if (status == FilterStatus.Match) return;
        if (Constants.excludeUsersIds.Contains(chatMessage.UserId)) return;
        
        var msg = new Message(chatMessage.Text, chatMessage.Username);
        Options.AddLog(msg);
        OnLogsAppended?.Invoke(chatMessage);
    }
    
    public List<Message> GetLogs() {
        return Options.GetLogs();
    }
}