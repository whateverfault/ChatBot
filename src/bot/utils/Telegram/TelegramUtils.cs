using System.Text;
using ChatBot.api.client;
using ChatBot.bot.services.logger;
using ChatBot.bot.utils.Telegram.Response;
using Newtonsoft.Json;

namespace ChatBot.bot.utils.Telegram;

public static class TelegramUtils {
    private static readonly HttpClient _httpClient = new HttpClient();
    
    
    public static async Task<SendMessageResponse?> SendMessageAsync(string botToken, long chatId, string message, bool disableNotification = false, LoggerService? logger = null) {
        try {
            var apiUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";
            
            var requestBody = new
                              {
                                  chat_id = chatId,
                                  text = message,
                                  disable_notification = disableNotification,
                              };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error performing a Telegram API call. Status: {response.StatusCode}. Content: {responseContent}");
                return null;
            }
            
            logger?.Log(LogLevel.Info, $"Telegram Message sent successfully. (ChatId - {chatId})");
            var deserialized = JsonConvert.DeserializeObject<SendMessageResponse>(responseContent);
            return deserialized;
        }
        catch (Exception ex) {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
        return null;
    }
    
    public static async Task<bool> DeleteMessageAsync(string botToken, long chatId, long messageId, LoggerService? logger = null) {
        try {
            var apiUrl = $"https://api.telegram.org/bot{botToken}/deleteMessage";
        
            var requestBody = new
                              {
                                  chat_id = chatId,
                                  message_id = messageId,
                              };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error deleting a Telegram message (chatId: {chatId}; id: {messageId}). Status: {response.StatusCode}. Content: {responseContent}");
                return false;
            }
        
            logger?.Log(LogLevel.Info, $"Telegram Message deleted successfully. (ChatId - {chatId}, MessageId - {messageId})");
            return true;
        }
        catch (Exception ex) {
            logger?.Log(LogLevel.Error, $"Caught an exception: {ex.Message}");
            return false;
        }
    }
}