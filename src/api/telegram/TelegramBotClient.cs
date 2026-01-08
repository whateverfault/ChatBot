using System.Net;
using System.Text;
using ChatBot.api.telegram.requests;
using ChatBot.api.telegram.response;
using Newtonsoft.Json;

namespace ChatBot.api.telegram;

public class TelegramBotClient {
    private static readonly SocketsHttpHandler _httpHandler = new SocketsHttpHandler
                                                              {
                                                                  PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                                                                  PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                                                                  MaxConnectionsPerServer = 50,
                                                                  UseCookies = false,
                                                                  AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                                                              };
    private readonly HttpClient _httpClient;

    private string _token;
    private long _chatId;
    
    
    public TelegramBotClient(string token, long chatId, HttpClient? client = null) {
        _token = token;
        _chatId = chatId;

        _httpClient = client ?? new HttpClient(_httpHandler);
    }

    public void UpdateToken(string token) {
        _token = token;
    }

    public void UpdateChatId(long chatId) {
        _chatId = chatId;
    }
    
    public async Task<SendMessageResponse?> SendMessageAsync(string message, EventHandler<string>? callback = null) {
        try {
            var apiUrl = $"https://api.telegram.org/bot{_token}/sendMessage";

            var request = new SendMessageRequest(
                                                 message,
                                                 _chatId
                                                 );

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(this, $"Error performing a Telegram API call. Status: {response.StatusCode}. Content: {responseContent}");
                return null;
            }
            
            var deserialized = JsonConvert.DeserializeObject<SendMessageResponse>(responseContent);
            return deserialized;
        }
        catch (Exception e) {
            callback?.Invoke(this, $"Error sending message: {e.Message}");
        }
        return null;
    }
    
    public async Task<bool> DeleteMessageAsync(long messageId, EventHandler<string>? callback = null) {
        try {
            var apiUrl = $"https://api.telegram.org/bot{_token}/deleteMessage";

            var request = new DeleteMessageRequest(
                                                   messageId,
                                                   _chatId
                                                   );

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(this, $"Error deleting a Telegram message. Status: {response.StatusCode}. Content: {responseContent}");
                return false;
            }
            return true;
        }
        catch (Exception ex) {
            callback?.Invoke(this, $"Caught an exception: {ex.Message}");
            return false;
        }
    }
}