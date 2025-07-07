using ChatBot.Services.logger;
using ChatBot.utils.Telegram;
using ChatBot.utils.Telegram.Response;

namespace ChatBot.Services.telegram;

public class TgBotClient {
    private readonly string _botToken;
    private readonly long _chatId;


    public TgBotClient(string botToken, long chatId) {
        _botToken = botToken;
        _chatId = chatId;
    }

    public async Task<SendMessageResponse?> SendMessageAsync(string message, bool disableNotification = false, LoggerService? logger = null) {
        return await TelegramUtils.SendMessageAsync(_botToken, _chatId, message, disableNotification, logger);
    }
    
    public async Task<bool> DeleteMessageAsync(int messageId, LoggerService? logger = null) {
        return await TelegramUtils.DeleteMessageAsync(_botToken, _chatId, messageId, logger);
    }
}