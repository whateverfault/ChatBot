using ChatBot.bot.services.logger;
using ChatBot.bot.utils.Telegram;
using ChatBot.bot.utils.Telegram.Response;

namespace ChatBot.bot.services.telegram;

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