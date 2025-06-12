using ChatBot.bot.interfaces;
using ChatBot.Services.chat_logs;
using ChatBot.Services.interfaces;
using ChatBot.Services.Static;

namespace ChatBot.Services.message_randomizer;

public class MessageRandomizerEvents : ServiceEvents {
    private MessageRandomizerService _service = null!;


    public override void Init(Service service, Bot bot) {
        _service = (MessageRandomizerService)service;
    }

    public override void Subscribe() {
        if (subscribed) return;
        base.Subscribe();
        var chatLogsService = (ChatLogsService)ServiceManager.GetService(ServiceName.ChatLogs);
        chatLogsService.OnLogsAppended += _service.HandleChatLog;
    }
}