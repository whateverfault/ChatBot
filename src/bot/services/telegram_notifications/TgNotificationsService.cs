using ChatBot.api.telegram;
using ChatBot.bot.chat_bot;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared;
using TwitchAPI.client;
using TwitchAPI.helix.data.requests;

namespace ChatBot.bot.services.telegram_notifications;

public class TgNotificationsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private static TwitchChatBot Bot => TwitchChatBot.Instance;

    private TelegramBotClient? _botClient;
    
    public override string Name => ServiceName.TgNotifications;
    public override TgNotificationsOptions Options { get; } = new TgNotificationsOptions();


    public async Task<long?> SendNotification(StreamData? data) {
        try {
            if (_botClient == null) return -1;
            
            var processed = ProcessPrompt(Options.NotificationPrompt, data);
            var response = await _botClient.SendMessageAsync(processed, (_, message) => {
                                                                         _logger.Log(LogLevel.Error, message);
                                                                     });

            if (response is not { Ok: true, }) {
                return null;
            }

            var messageId = response.Result.MessageId;
            _logger.Log(LogLevel.Info, $"Telegram notification is sent. (id: {messageId})");
            return messageId;
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while sending a telegram notification message: {e.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteNotification(long messageId) {
        try {
            if (_botClient == null) return false;
            
            _logger.Log(LogLevel.Info, $"Previous telegram notification message has been deleted. (id: {messageId})");
            return await _botClient.DeleteMessageAsync(messageId, (_, message) => {
                                                                              _logger.Log(LogLevel.Error, message);
                                                                          });
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while deleting a telegram message: {e.Message}");
        }
        return false;
    }
    
    private string ProcessPrompt(string prompt, StreamData? data) {
        var random = Random.Shared;
        var client = Bot.GetClient();
        
        var replacements = new Dictionary<string, string> {
                                                              {
                                                                  "{title}",
                                                                  $"{data?.Title ?? string.Empty}"
                                                              },
                                                              {
                                                                  "{link}",
                                                                  $"{Constants.BaseTwitchUrl}{client?.Credentials?.Channel ?? string.Empty}?v={random.Next(int.MinValue, int.MaxValue)}"
                                                              },
                                                              {
                                                                  "\\n",
                                                                  "\n"
                                                              },
                                                          };
        
        foreach (var (key, replacement) in replacements) {
            prompt = prompt.Replace(key, replacement);
        }

        return prompt;
    }

    public void SetBotToken(string token) {
        Options.SetBotToken(token);
        _botClient?.UpdateToken(Options.BotToken);
    }
    
    public void SetChatId(long chatId) {
        Options.SetChatId(chatId);
        _botClient?.UpdateChatId(Options.ChatId);
    }
    
    public void SetNotificationPrompt(string prompt) {
        Options.SetNotificationPrompt(prompt);
    }
    
    public string GetBotToken() {
        return Options.BotToken;
    }
    
    public long GetChatId() {
        return Options.ChatId;
    }
    
    public string GetNotificationPrompt() {
        return Options.NotificationPrompt;
    }
    
    public long GetCooldown() {
        return Options.Cooldown;
    }

    public bool GetIsSent() {
        return Options.IsSent;
    }
    
    public long? GetLastSentTime() {
        return Options.LastSent;
    }
    
    public override void Init() {
        base.Init();
        
        _botClient = new TelegramBotClient(Options.BotToken, Options.ChatId);
    }
}