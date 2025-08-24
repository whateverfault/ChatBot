using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ChatBot.api.twitch.client.credentials;
using ChatBot.api.twitch.event_sub.subscription_data.subscription;
using ChatBot.api.twitch.helix.data.requests.chat_subscription;
using Newtonsoft.Json;

namespace ChatBot.api.twitch.event_sub;

public static class EventSub {
    private static readonly SocketsHttpHandler _httpHandler = new SocketsHttpHandler
                                                              {
                                                                  PooledConnectionLifetime  = TimeSpan.FromMinutes(2),
                                                                  MaxConnectionsPerServer = 50,
                                                                  EnableMultipleHttp2Connections = true,
                                                                  PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
                                                                  UseCookies = false,
                                                                  AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                                                              };
    private static readonly HttpClient _httpClient = new HttpClient(_httpHandler);
    
    
    public static async Task<EventSubPayload?> SubscribeToChannelChat(string? sessionId, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var subscription = new EventSubPayload(
                                                   "channel.chat.message",
                                                   "1",
                                                   new Condition(credentials.ChannelId, credentials.UserId),
                                                   new Transport("websocket", sessionId)
                                                  );
            
            var json = JsonConvert.SerializeObject(subscription);
            
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitch.tv/helix/eventsub/subscriptions");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.Oauth);
            request.Headers.Add("Client-Id", credentials.ClientId);
            
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null, $"Couldn't subscribe to chat. Status: {response.StatusCode}. Content: {content}");
                return null;
            }
            
            var deserialized = JsonConvert.DeserializeObject<EventSubData>(content);
            return deserialized?.Data.FirstOrDefault();
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while subscribing to a chat. {e.Message}");
            return null;
        }
    }
    
    public static async Task EventSubUnSubscribe(string subscriptionId, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"https://api.twitch.tv/helix/eventsub/subscriptions?id={subscriptionId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.Oauth);
            request.Headers.Add("Client-Id", credentials.ClientId);
            
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null, $"Couldn't unsubscribe from EventSub. Status: {response.StatusCode}. Content: {responseContent}");
            }
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while unsubscribing from EventSub. {e.Message}");
        }
    }
}