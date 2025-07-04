using System.Net.Http.Headers;
using System.Text;
using ChatBot.bot;
using ChatBot.Services.logger;
using ChatBot.Services.Static;
using ChatBot.utils.Twitch.Helix.Data;
using ChatBot.utils.Twitch.Helix.Responses;
using Newtonsoft.Json;
using TwitchLib.Client.Models;
using LogLevel = ChatBot.Services.logger.LogLevel;

namespace ChatBot.utils.Twitch.Helix;

public static class HelixUtils {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private static readonly HttpClient _httpClient = new();
    
    
    #region ban user
    public static async Task BanUserHelix(ChatBotOptions options, string username, string message) {
        try {
            var userId = await TwitchLibUtils.GetUserId(options, username);
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            var botId = await TwitchLibUtils.GetUserId(options, options.Username!);
            if (string.IsNullOrEmpty(userId)) {
                _logger.Log(LogLevel.Error, $"User {username} not found");
                return;
            } if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"User {options.Channel!} not found");
                return;
            } if (string.IsNullOrEmpty(botId)) {
                _logger.Log(LogLevel.Error, $"User {options.Username!} not found");
                return;
            }
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);
            
            var request = new
                          {
                              data = new
                                     {
                                         user_id = userId,
                                         reason = message
                                     }
                          };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={broadcasterId}&moderator_id={botId}", content);

            if (response.IsSuccessStatusCode) {
                Console.WriteLine();
                _logger.Log(LogLevel.Error, $"Successfully banned {username}. Message: {message}");
            } else {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Log(LogLevel.Error, $"Failed to ban {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error banning user: {ex.Message}");
        }
    }
    #endregion
    
    #region timeout
    public static async Task TimeoutUserHelix(ChatBotOptions options, string username, TimeSpan durationSeconds, string message) {
        try {
            var userId = await TwitchLibUtils.GetUserId(options, username);
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            var botId = await TwitchLibUtils.GetUserId(options, options.Username!);
            if (string.IsNullOrEmpty(userId)) {
                Console.WriteLine($"User {username} not found");
                return;
            } if (string.IsNullOrEmpty(broadcasterId)) {
                Console.WriteLine($"User {options.Channel!} not found");
                return;
            } if (string.IsNullOrEmpty(botId)) {
                Console.WriteLine($"User {options.Username!} not found");
                return;
            } 

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);
            
            var request = new
                          {
                              data = new
                                     {
                                         user_id = userId,
                                         duration = (int)durationSeconds.TotalSeconds,
                                         reason = message
                                     }
                          };

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Post,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={broadcasterId}&moderator_id={botId}"),
                                     Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.OAuth}" }
                                     }
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode) {
                _logger.Log(LogLevel.Error, $"Successfully timed out {username} for {(int)durationSeconds.TotalSeconds} seconds. Reason: {message}");
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Log(LogLevel.Error, $"Failed to timeout {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error timing out user: {ex.Message}");
        }
    }
    #endregion
    
    #region delete message
    public static async Task DeleteMessageHelix(ChatBotOptions options, ChatMessage message) {
        try {
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            var botId = await TwitchLibUtils.GetUserId(options, options.Username!);
            var userId = await TwitchLibUtils.GetUserId(options, message.Username);
            
            if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"Channel {options.Channel!} not found");
                return;
            } if (string.IsNullOrEmpty(botId)) {
                _logger.Log(LogLevel.Error, $"User {options.Username!} not found");
                return;
            } if (string.IsNullOrEmpty(userId)) {
                _logger.Log(LogLevel.Error, $"User {message.Username} not found");
                return;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);
            
            var requestUri = $"https://api.twitch.tv/helix/moderation/chat?broadcaster_id={broadcasterId}&moderator_id={botId}&message_id={message.Id}&user_id={userId}";
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Delete,
                                     RequestUri = new Uri(requestUri),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.OAuth}" }
                                     }
                                 };
            var response = await _httpClient.SendAsync(requestMessage);
            
            if (response.IsSuccessStatusCode) {
                _logger.Log(LogLevel.Info, $"Successfully deleted message {message.Id ?? $"from user {message.Username}"}");
            } else {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Log(LogLevel.Error, $"Failed to delete message. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error deleting message: {ex.Message}");
        }
    }
    #endregion
    
    #region followage
    public static async Task<TimeSpan?> GetFollowageHelix(ChatBotOptions options, string username) {
        try {
            var userId = await TwitchLibUtils.GetUserId(options, username);
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            
            if (string.IsNullOrEmpty(userId)) {
                _logger.Log(LogLevel.Error, $"User {username} not found");
                return null;
            }
            if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"Channel {options.Channel!} not found");
                return null;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);
            
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.twitch.tv/helix/channels/followers?user_id={userId}&broadcaster_id={broadcasterId}"),
                Headers =
                {
                    { "Client-ID", options.ClientId },
                    { "Authorization", $"Bearer {options.OAuth}" }
                }
            };

            var response = await _httpClient.SendAsync(requestMessage);
            
            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                var followData = JsonConvert.DeserializeObject<FollowResponse>(responseContent);
                
                if (followData?.Data?.Count > 0) {
                    var followDate = followData.Data[0].FollowedAt;
                    var followDuration = DateTime.UtcNow - followDate;
                    _logger.Log(LogLevel.Info, $"{username} has been following since {followDate} ({followDuration.TotalDays} days)");
                    return followDuration;
                }
                _logger.Log(LogLevel.Info, $"{username} is not following {options.Channel}");
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Log(LogLevel.Error, $"Failed to get followage for {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
            return null;
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error getting followage: {ex.Message}");
            return null;
        } 
    }
    #endregion
    
    #region channel info
    
    public static async Task<bool> UpdateChannelInfo(ChatBotOptions options, string newTitle, string newGameId) {
        try {
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"Channel {options.Channel!} not found");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.BroadcasterOAuth);

            var requestBody = new 
                              {
                                      game_id = newGameId,
                                      title = newTitle,
                              };
            var jsonContent = JsonConvert.SerializeObject(requestBody);

            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Patch,
                                                            RequestUri = new Uri($"https://api.twitch.tv/helix/channels?broadcaster_id={broadcasterId}"),
                                                            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                                                            Headers =
                                                            {
                                                                { "Client-ID", options.ClientId },
                                                                { "Authorization", $"Bearer {options.BroadcasterOAuth}" }
                                                            } 
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                _logger.Log(LogLevel.Info, $"Successfully updated channel info: Title='{newTitle}', GameID='{newGameId}'");
                return true;
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.Log(LogLevel.Error, $"Failed to update channel info. Status: {response.StatusCode}. Response: {responseContent}");
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error updating channel info: {ex.Message}");
        }
        return false;
    }

    public static async Task<ChannelInfo?> GetChannelInfo(ChatBotOptions options) {
        try {
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"Channel {options.Channel!} not found");
                return null;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/channels?broadcaster_id={broadcasterId}"),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.OAuth}" }
                                     }
                                 };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                var channelData = JsonConvert.DeserializeObject<ChannelInfoResponse>(responseContent);
                _logger.Log(LogLevel.Info, "Successfully fetched channel info");
                return channelData?.Data?.FirstOrDefault();
            }
        
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.Log(LogLevel.Error, $"Failed to get channel info. Status: {response.StatusCode}. Response: {errorContent}");
            return null;
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error getting channel info: {ex.Message}");
            return null;
        }
    }
    
    public static async Task<string?> FindGameId(ChatBotOptions options, string searchQuery) {
        try {
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);

            var exactMatch = await SearchSingleGame(options, searchQuery);
            if (exactMatch != null) return exactMatch.Id;

            var encodedQuery = Uri.EscapeDataString(searchQuery);
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/search/categories?query={encodedQuery}&first=5"),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.OAuth}" }
                                     }
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
            _logger.Log(LogLevel.Error, $"Error searching for game: {ex.Message}");
            return null;
        }
    }
    
    private static async Task<GameData?> SearchSingleGame(ChatBotOptions options, string gameName) {
        try {
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);

            var encodedGameName = Uri.EscapeDataString(gameName);
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/games?name={encodedGameName}"),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.OAuth}" }
                                     }
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
    #endregion

    #region channel points
    
    public static async Task<bool> SetChannelRewardState(ChatBotOptions options, string rewardId, bool state) { 
        try {
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"Channel {options.Channel!} not found");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.BroadcasterOAuth);

            var requestBody = new 
                              {
                                  is_enabled = state
                              };
            var jsonContent = JsonConvert.SerializeObject(requestBody);

            var requestMessage = new HttpRequestMessage 
                                 {
                                     Method = HttpMethod.Patch,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={broadcasterId}&id={rewardId}"),
                                     Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.BroadcasterOAuth}" }
                                     }
                                 };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                _logger.Log(LogLevel.Info, $"Successfully changed state of a reward with ID: {rewardId}");
                return true;
            }
        
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.Log(LogLevel.Error, $"Failed to change state of a reward. Status: {response.StatusCode}. Response: {responseContent}");
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error changing state of a reward: {ex.Message}");
        }
        return false;
    }

    public static async Task<string?> CreateChannelReward(
    ChatBotOptions options,
    string title,
    int cost,
    string? prompt = null,
    bool isEnabled = true,
    string? backgroundColor = null,
    bool userInputRequired = false,
    bool skipQueue = false) 
    {
        try {
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"Channel {options.Channel!} not found");
                return null;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.BroadcasterOAuth);

            var requestBody = new 
                              {
                                  title,
                                  cost,
                                  prompt,
                                  is_enabled = isEnabled,
                                  background_color = backgroundColor,
                                  is_user_input_required = userInputRequired,
                                  should_redemptions_skip_request_queue = skipQueue
                              };

            var jsonContent = JsonConvert.SerializeObject(requestBody);

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Post,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={broadcasterId}"),
                                     Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                                 };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                var rewardResponse = JsonConvert.DeserializeObject<RewardCreationResponse>(responseContent);
            
                _logger.Log(LogLevel.Info, $"Successfully created reward: {title} (Cost: {cost})");
                return rewardResponse?.Data?.FirstOrDefault()?.Id;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.Log(LogLevel.Error, $"Failed to create reward. Status: {response.StatusCode}. Response: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Error creating reward: {ex.Message}");
        }
        return null;
    }
    
    public static async Task<bool> DeleteChannelReward(ChatBotOptions options, string rewardId) {
        try {
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"Channel {options.Channel!} not found");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.BroadcasterOAuth);

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Delete,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={broadcasterId}&id={rewardId}")
                                 };

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) {
                _logger.Log(LogLevel.Info, $"Successfully deleted reward ID: {rewardId}");
                return true;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.Log(LogLevel.Error, $"Failed to delete reward. Status: {response.StatusCode}. Response: {responseContent}");
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error deleting reward: {ex.Message}");
        }
        return false;
    }
    
    #endregion
    
    #region create clip
    public static async Task<string?> CreateClipHelix(ChatBotOptions options) {
        try {
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            if (string.IsNullOrEmpty(broadcasterId)) {
                Console.WriteLine($"User {options.Channel!} not found");
                return null;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Post,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/clips?broadcaster_id={broadcasterId}"),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.OAuth}" }
                                     }
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                var clipResponse = JsonConvert.DeserializeObject<ClipCreationResponse>(responseContent);
            
                if (clipResponse?.Data?.Count > 0) {
                    var clipId = clipResponse.Data[0].Id;
                    _logger.Log(LogLevel.Info, $"Successfully created clip with ID: {clipId}");
                    return clipId;
                }
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Log(LogLevel.Error, $"Failed to create clip. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error creating clip: {ex.Message}");
        }

        return null;
    }
    #endregion
}