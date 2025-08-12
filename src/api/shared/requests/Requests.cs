using System.Net.Http.Headers;
using System.Text;
using ChatBot.api.client;
using ChatBot.api.client.credentials;
using ChatBot.api.event_sub.subscription_data;
using ChatBot.api.event_sub.subscription_data.subscription;
using ChatBot.api.shared.requests.data;
using ChatBot.api.shared.requests.data.ChatSubscriptionRequest;
using ChatBot.api.shared.responses;
using ChatBot.api.shared.responses.GetUserInfo;
using ChatBot.api.shared.responses.SendMessage;
using Newtonsoft.Json;
using ChatMessage = ChatBot.api.client.data.ChatMessage;

namespace ChatBot.api.shared.requests;

public static class Requests {
    private static readonly HttpClient _httpClient = new HttpClient();


    public static async Task<ValidateResponse?> ValidateOauth(string oauth, EventHandler<string>? callback = null) {
        try {
            if (string.IsNullOrEmpty(oauth)) {
                callback?.Invoke(null, "Oauth token is empty.");
                return null;
            }
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", oauth);

            var response = await _httpClient.GetAsync("https://id.twitch.tv/oauth2/validate");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null, $"Failed to validate oauth token. Status: {response.StatusCode}. Content: {responseContent}");
                return null;
            }
            
            var deserialized = JsonConvert.DeserializeObject<ValidateResponse>(responseContent);
            return deserialized;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Exception while validating an oauth token. {e.Message}");
            return null;
        }
    }

    public static async Task<UserInfo?> GetUserInfo(string username, string oauth, string clientId, EventHandler<string>? callback = null) {
        try {
            if (string.IsNullOrEmpty(username)) {
                callback?.Invoke(null, "Username is empty.");
                return null;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", oauth);
            _httpClient.DefaultRequestHeaders.Add("Client-Id", clientId);

            var response = await _httpClient.GetAsync($"https://api.twitch.tv/helix/users?login={username}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Failed to get a user id. Status: {response.StatusCode}. Content: {responseContent}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<GetUserInfoResponse>(responseContent);

            if (deserialized == null
             || deserialized.Data.Length <= 0) {
                return null;
            }

            return deserialized.Data[0];
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Exception while getting a user id. {e.Message}");
            return null;
        }
    }

    public static async Task<SendMessageResponse?> SendMessage(string message, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            if (string.IsNullOrEmpty(message)) {
                callback?.Invoke(null, "Message is empty.");
                return null;
            }
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", credentials.OAuth);
            _httpClient.DefaultRequestHeaders.Add("Client-Id", credentials.ClientId);

            var request = new SendMessagePayload(credentials.ChannelId,
                                                 credentials.UserId,
                                                 message);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("https://api.twitch.tv/helix/chat/messages", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null, $"Failed to send a message. Status: {response.StatusCode}. Content: {responseContent}");
                return null;
            }
            
            var deserialized = JsonConvert.DeserializeObject<SendMessageResponse>(responseContent);
            return deserialized;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Exception while sending a message. {e.Message}");
            return null;
        }
    }
    
    public static async Task<SendMessageResponse?> SendReply(string message, string replyId, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            if (string.IsNullOrEmpty(message)) {
                callback?.Invoke(null, "Message is empty.");
                return null;
            }
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", credentials.OAuth);
            _httpClient.DefaultRequestHeaders.Add("Client-Id", credentials.ClientId);

            var request = new SendReplyPayload(credentials.ChannelId,
                                               credentials.UserId,
                                               message,
                                               replyId);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("https://api.twitch.tv/helix/chat/messages", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null, $"Failed to send a message. Status: {response.StatusCode}. Content: {responseContent}");
                return null;
            }
            
            var deserialized = JsonConvert.DeserializeObject<SendMessageResponse>(responseContent);
            return deserialized;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Exception while sending a message. {e.Message}");
            return null;
        }
    }
    
    public static async Task<ChatSubscriptionPayload?> SubscribeToChannelChat(string? sessionId, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);
            _httpClient.DefaultRequestHeaders.Add("Client-Id", credentials.ClientId);

            var subscription = new ChatSubscriptionPayload(
                                                           "channel.chat.message",
                                                           "1",
                                                           new Condition(credentials.ChannelId, credentials.UserId),
                                                           new Transport("websocket", sessionId)
                                                           );

            var json = JsonConvert.SerializeObject(subscription);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
        
            var response = await _httpClient.PostAsync("https://api.twitch.tv/helix/eventsub/subscriptions", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null, $"Couldn't subscribe to a chat. Status: {response.StatusCode}. Content: {responseContent}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<ChatSubscriptionData>(responseContent);

            if (deserialized == null || deserialized.Data.Length <= 0) {
                return null;
            }
            
            return deserialized.Data[0];
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while subscribing to a chat. {e.Message}");
        }
        return null;
    }
    
    public static async Task UnsubscribeFromChannelChat(string subscriptionId, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);
            _httpClient.DefaultRequestHeaders.Add("Client-Id", credentials.ClientId);

            var response = await _httpClient.DeleteAsync($"https://api.twitch.tv/helix/eventsub/subscriptions?id={subscriptionId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null, $"Couldn't unsubscribe from chat. Status: {response.StatusCode}. Content: {responseContent}");
            }
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while unsubscribing from chat. {e.Message}");
        }
    }
    
    public static async Task BanUser(string username, string message, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var userId = await GetUserId(username, credentials.OAuth, credentials.ClientId, callback);
            if (userId == null) return;
            
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return;
            
            var botId = await GetUserId(credentials.Username, credentials.OAuth, credentials.ClientId, callback);
            if (botId == null) return;
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);
            
            var request = new
                          {
                              data = new
                                     {
                                         user_id = userId,
                                         reason = message,
                                     },
                          };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={channelId}&moderator_id={botId}", content);

            if (response.IsSuccessStatusCode) {
                Console.WriteLine();
                callback?.Invoke(null, $"Successfully banned {username}. Message: {message}");
            } else {
                var responseContent = await response.Content.ReadAsStringAsync();
                callback?.Invoke(null, $"Failed to ban {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error banning user: {ex.Message}");
        }
    }
    
        public static async Task TimeoutUserHelix(string username, string message, TimeSpan durationSeconds, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var userId = await GetUserId(username, credentials.OAuth, credentials.ClientId, callback);
            if (userId == null) return;
            
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return;
            
            var botId = await GetUserId(credentials.Username, credentials.OAuth, credentials.ClientId, callback);
            if (botId == null) return;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);
            
            var request = new
                          {
                              data = new
                                     {
                                         user_id = userId,
                                         duration = (int)durationSeconds.TotalSeconds,
                                         reason = message,
                                     },
                          };

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Post,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={channelId}&moderator_id={botId}"),
                                     Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
                                     Headers =
                                     {
                                         { "Client-ID", credentials.ClientId },
                                         { "Authorization", $"Bearer {credentials.OAuth}" },
                                     },
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode) {
                callback?.Invoke(null, $"Successfully timed out {username} for {(int)durationSeconds.TotalSeconds} seconds. Reason: {message}");
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                callback?.Invoke(null, $"Failed to timeout {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error timing out user: {ex.Message}");
        }
    }
    
    public static async Task DeleteMessage(ChatMessage message, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var userId = await GetUserId(message.Username, credentials.OAuth, credentials.ClientId, callback);
            if (userId == null) return;
            
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return;
            
            var botId = await GetUserId(credentials.Username, credentials.OAuth, credentials.ClientId, callback);
            if (botId == null) return;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);
            
            var requestUri = $"https://api.twitch.tv/helix/moderation/chat?broadcaster_id={channelId}&moderator_id={botId}&message_id={message.Id}&user_id={userId}";
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Delete,
                                     RequestUri = new Uri(requestUri),
                                     Headers =
                                     {
                                         { "Client-ID", credentials.ClientId },
                                         { "Authorization", $"Bearer {credentials.OAuth}" },
                                     },
                                 };
            var response = await _httpClient.SendAsync(requestMessage);
            
            if (response.IsSuccessStatusCode) {
                callback?.Invoke(LogLevel.Info, $"Successfully deleted message {message.Id}");
            } else {
                var responseContent = await response.Content.ReadAsStringAsync();
                callback?.Invoke(null, $"Failed to delete message. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error deleting message: {ex.Message}");
        }
    }
    
    public static async Task<TimeSpan?> GetFollowageHelix(string username, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var userId = await GetUserId(credentials.Username, credentials.OAuth, credentials.ClientId, callback);
            if (userId == null) return null;
            
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return null;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);
            
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.twitch.tv/helix/channels/followers?user_id={userId}&broadcaster_id={channelId}"),
                Headers =
                {
                    { "Client-ID", credentials.ClientId },
                    { "Authorization", $"Bearer {credentials.OAuth}" },
                },
            };

            var response = await _httpClient.SendAsync(requestMessage);
            
            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                var followData = JsonConvert.DeserializeObject<FollowResponse>(responseContent);
                
                if (followData?.Data?.Count > 0) {
                    var followDate = followData.Data[0].FollowedAt;
                    var followDuration = DateTime.UtcNow - followDate;
                    callback?.Invoke(LogLevel.Info, $"{username} has been following since {followDate} ({followDuration.TotalDays} days)");
                    return followDuration;
                }
                callback?.Invoke(LogLevel.Info, $"{username} is not following {credentials.Channel}");
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                callback?.Invoke(null, $"Failed to get followage for {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
            return null;
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error getting followage: {ex.Message}");
            return null;
        } 
    }
    
    public static async Task<bool> UpdateChannelInfo(string newTitle, string newGameId, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return false;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.ChannelOAuth);

            var requestBody = new 
                              {
                                      game_id = newGameId,
                                      title = newTitle,
                              };
            var jsonContent = JsonConvert.SerializeObject(requestBody);

            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Patch,
                                                            RequestUri = new Uri($"https://api.twitch.tv/helix/channels?broadcaster_id={channelId}"),
                                                            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                                                            Headers =
                                                            {
                                                                { "Client-ID", credentials.ClientId },
                                                                { "Authorization", $"Bearer {credentials.ChannelOAuth}" },
                                                            },
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                callback?.Invoke(LogLevel.Info, $"Successfully updated channel info: Title='{newTitle}', GameID='{newGameId}'");
                return true;
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            callback?.Invoke(null, $"Failed to update channel info. Status: {response.StatusCode}. Response: {responseContent}");
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error updating channel info: {ex.Message}");
        }
        return false;
    }

    public static async Task<ChannelInfo?> GetChannelInfo(FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return null;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/channels?broadcaster_id={channelId}"),
                                     Headers =
                                     {
                                         { "Client-ID", credentials.ClientId },
                                         { "Authorization", $"Bearer {credentials.OAuth}" },
                                     },
                                 };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                var channelData = JsonConvert.DeserializeObject<ChannelInfoResponse>(responseContent);
                callback?.Invoke(LogLevel.Info, "Successfully fetched channel info");
                return channelData?.Data?.FirstOrDefault();
            }
        
            var errorContent = await response.Content.ReadAsStringAsync();
            callback?.Invoke(null, $"Failed to get channel info. Status: {response.StatusCode}. Response: {errorContent}");
            return null;
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error getting channel info: {ex.Message}");
            return null;
        }
    }
    
    public static async Task<string?> FindGameId(string searchQuery, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);

            var exactMatch = await SearchSingleGame(searchQuery, credentials);
            if (exactMatch != null) return exactMatch.Id;

            var encodedQuery = Uri.EscapeDataString(searchQuery);
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/search/categories?query={encodedQuery}&first=5"),
                                     Headers =
                                     {
                                         { "Client-ID", credentials.ClientId },
                                         { "Authorization", $"Bearer {credentials.OAuth}" },
                                     },
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var searchResults = JsonConvert.DeserializeObject<GameSearchResponse>(responseContent);
            if (!(searchResults?.Data?.Count > 0)) return null;
            
            var bestMatch = searchResults.Data
                                      .OrderBy(g => CalculateLevenshteinDistance(g.Name.ToLower(), searchQuery.ToLower()))
                                      .First();

            return bestMatch.Id;
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error searching for game: {ex.Message}");
            return null;
        }
    }
    
    private static async Task<GameData?> SearchSingleGame(string gameName, FullCredentials credentials) {
        try {
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);

            var encodedGameName = Uri.EscapeDataString(gameName);
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/games?name={encodedGameName}"),
                                     Headers =
                                     {
                                         { "Client-ID", credentials.ClientId },
                                         { "Authorization", $"Bearer {credentials.OAuth}" },
                                     },
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<GameSearchResponse>(content);
            return result?.Data?.FirstOrDefault();
        }
        catch {
            return null;
        }
    }
    
    private static int CalculateLevenshteinDistance(string a, string b) {
        if (string.IsNullOrEmpty(a)) return string.IsNullOrEmpty(b) ? 0 : b.Length;
        if (string.IsNullOrEmpty(b)) return a.Length;

        var lengthA = a.Length;
        var lengthB = b.Length;
        var distances = new int[lengthA + 1, lengthB + 1];

        for (var i = 0; i <= lengthA; distances[i, 0] = i++);
        for (var j = 0; j <= lengthB; distances[0, j] = j++);

        for (var i = 1; i <= lengthA; i++) {
            for (var j = 1; j <= lengthB; j++) {
                var cost = b[j - 1] == a[i - 1] ? 0 : 1;
                distances[i, j] = Math.Min(
                                           Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                                           distances[i - 1, j - 1] + cost
                                           );
            }
        }
        return distances[lengthA, lengthB];
    }
    
    public static async Task<bool> SetChannelRewardState(string rewardId, bool state, FullCredentials credentials, EventHandler<string>? callback = null) { 
        try {
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return false;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.ChannelOAuth);

            var requestBody = new 
                              {
                                  is_enabled = state,
                              };
            var jsonContent = JsonConvert.SerializeObject(requestBody);

            var requestMessage = new HttpRequestMessage 
                                 {
                                     Method = HttpMethod.Patch,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={channelId}&id={rewardId}"),
                                     Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                                     Headers =
                                     {
                                         { "Client-ID", credentials.ClientId },
                                         { "Authorization", $"Bearer {credentials.ChannelOAuth}" },
                                     },
                                 };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                callback?.Invoke(LogLevel.Info, $"Successfully changed state of a reward with ID: {rewardId}");
                return true;
            }
        
            var responseContent = await response.Content.ReadAsStringAsync();
            callback?.Invoke(null, $"Failed to change state of a reward. Status: {response.StatusCode}. Response: {responseContent}");
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error changing state of a reward: {ex.Message}");
        }
        return false;
    }

    public static async Task<string?> CreateChannelReward(
    string title,
    int cost,
    FullCredentials credentials,
    string? prompt = null,
    bool isEnabled = true,
    string? backgroundColor = null,
    bool userInputRequired = false,
    bool skipQueue = false, 
    EventHandler<string>? callback = null) {
        try {
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return null;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.ChannelOAuth);

            var requestBody = new 
                              {
                                  title,
                                  cost,
                                  prompt,
                                  is_enabled = isEnabled,
                                  background_color = backgroundColor,
                                  is_user_input_required = userInputRequired,
                                  should_redemptions_skip_request_queue = skipQueue,
                              };

            var jsonContent = JsonConvert.SerializeObject(requestBody);

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Post,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={channelId}"),
                                     Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                var rewardResponse = JsonConvert.DeserializeObject<RewardCreationResponse>(responseContent);
            
                callback?.Invoke(LogLevel.Info, $"Successfully created reward: {title} (Cost: {cost})");
                return rewardResponse?.Data.FirstOrDefault()?.Id;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            callback?.Invoke(null, $"Failed to create reward. Status: {response.StatusCode}. Response: {errorContent}");
        }
        catch (Exception ex)
        {
            callback?.Invoke(null, $"Error creating reward: {ex.Message}");
        }
        return null;
    }
    
    public static async Task<bool> DeleteChannelReward(string rewardId, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return false;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.ChannelOAuth);

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Delete,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={channelId}&id={rewardId}"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                callback?.Invoke(LogLevel.Info, $"Successfully deleted reward ID: {rewardId}");
                return true;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            callback?.Invoke(null, $"Failed to delete reward. Status: {response.StatusCode}. Response: {responseContent}");
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error deleting reward: {ex.Message}");
        }
        return false;
    }
    
    public static async Task<bool> SendWhisper(string userId, string message, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var botId = await GetUserId(credentials.Username, credentials.OAuth, credentials.ClientId, callback);
            if (botId == null) return false;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);

            var requestBody = new { 
                                      message,
                                  };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Post,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/whispers?from_user_id={botId}&to_user_id={userId}"),
                                     Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                return true;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            callback?.Invoke(null, $"Failed to send whisper. Status: {response.StatusCode}. Response: {responseContent}");
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error sending whisper: {ex.Message}");
        }
        return false;
    }
    
    public static async Task<string?> CreateClip(FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return null;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Post,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/clips?broadcaster_id={channelId}"),
                                     Headers =
                                     {
                                         { "Client-ID", credentials.ClientId },
                                         { "Authorization", $"Bearer {credentials.OAuth}" },
                                     },
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                var clipResponse = JsonConvert.DeserializeObject<ClipCreationResponse>(responseContent);
            
                if (clipResponse?.Data?.Count > 0) {
                    var clipId = clipResponse.Data[0].Id;
                    return clipId;
                }
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                callback?.Invoke(null, $"Failed to create clip. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Error creating clip: {ex.Message}");
        }

        return null;
    }
    public static async Task<StreamResponse?> GetStreams(string username, FullCredentials credentials, EventHandler<string>? callback = null) {
        try {
            var channelId = await GetUserId(credentials.Channel, credentials.OAuth, credentials.ClientId, callback);
            if (channelId == null) return null;
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", credentials.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials.OAuth);
        
            var response = await _httpClient.GetAsync($"https://api.twitch.tv/helix/streams?user_id={channelId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                var streamResponse = JsonConvert.DeserializeObject<StreamResponse>(responseContent);
                return streamResponse;
            }
            
            callback?.Invoke(null, $"Failed to get stream info for {username}. Status: {response.StatusCode}. Response: {responseContent}");
        }
        catch (Exception ex) {
            callback?.Invoke(null, $"Exception while getting stream info: {ex.Message}");
        }
        return null;
    }
    
    public static async Task<string?> GetUserId(string username, string oauth, string clientId, EventHandler<string>? callback = null) {
        var userInfo = await GetUserInfo(username, oauth, clientId);
        if (userInfo == null) {
            callback?.Invoke(null, $"Couldn't get info of user '{username}'");
            return null;
        }

        if (!string.IsNullOrEmpty(userInfo.Id)) {
            return userInfo.Id;
        }

        callback?.Invoke(null, $"User {username} not found");
        return null;
    }
    
    public static async Task<string?> GetUserId(string username, FullCredentials credentials, EventHandler<string>? callback = null) {
        var userInfo = await GetUserInfo(username, credentials.OAuth, credentials.ClientId);
        if (userInfo == null) {
            callback?.Invoke(null, $"Couldn't get info of user '{username}'");
            return null;
        }

        if (!string.IsNullOrEmpty(userInfo.Id)) {
            return userInfo.Id;
        }

        callback?.Invoke(null, $"User {username} not found");
        return null;
    }
}