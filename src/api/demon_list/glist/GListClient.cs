using System.Net;
using ChatBot.api.demon_list.data;
using ChatBot.api.demon_list.glist.responses;
using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist;

public class GListClient : IDemonList {
    private static readonly SocketsHttpHandler _httpHandler = new SocketsHttpHandler
                                                              {
                                                                  PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                                                                  PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                                                                  MaxConnectionsPerServer = 50,
                                                                  UseCookies = false,
                                                                  AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                                                                  UseProxy = true,
                                                                  Proxy = WebRequest.DefaultWebProxy,
                                                              };
    private readonly HttpClient _httpClient;
    private readonly GListCache? _cache;
    
    
    public GListClient(bool caching, HttpClient? client = null) {
        _httpClient = client ?? new HttpClient(_httpHandler);

        if (caching) {
            _cache = new GListCache();
        }
    }
    
    public void ResetCache() {
        _cache?.ResetCache();
    }

    public async Task<UserProfile?> GetProfile(string username, EventHandler<string>? errorCallback = null) {
        try {
            using var requestMessage = 
                new HttpRequestMessage(HttpMethod.Get, $"https://api.demonlist.org/leaderboard/user/list?limit=1&offset=0&search={username}");
            
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                      $"Error while getting Global List's user data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<GetUsersResponse>(content);
            if (deserialized == null || deserialized.Data.Users.Count <= 0) return null;
            
            var userData = deserialized.Data.Users[0];
            
            using var userRequestMessage = 
                new HttpRequestMessage(HttpMethod.Get, $"https://api.demonlist.org/user/get?id={userData.Id}");
            
            response = await _httpClient.SendAsync(userRequestMessage);
            content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                      $"Error while getting Global List's user profile. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
            
            var deserializedProfile = JsonConvert.DeserializeObject<GetUserProfileResponse>(content);
            var profile = deserializedProfile?.Data;
            if (profile?.Levels?.Hardest == null) return null;
            
            return new UserProfile(
                                   userData.Id.ToString(),
                                   profile.Placement,
                                   profile.Points,
                                   profile.Username,
                                   profile.Username,
                                   new LevelInfo(profile.Levels.Hardest));
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while getting Global List's user profile: {e.Message}");
            return null;
        }
    }

    public async Task<List<LevelInfo>?> ListLevels(EventHandler<string>? errorCallback = null) {
        try {
            if (_cache is { Levels: not null, }) {
                return _cache.Levels;
            }
            
            using var requestMessage = 
                new HttpRequestMessage(HttpMethod.Get, $"https://api.demonlist.org/level/classic/list?limit={int.MaxValue}&offset=0");
            
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                      $"Error while fetching list levels data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<ListLevelsResponse>(content);
            if (deserialized == null) return null;
            
            var result = deserialized.Data.Levels.Select(x => new LevelInfo(x)).ToList();
            _cache?.CacheLevelsList(result);
            return result;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching list levels data: {e.Message}");
            return null;
        }
    }
    
    public async Task<List<LevelInfo>?> GetLevelsByName(string name, EventHandler<string>? errorCallback = null) {
        try {
            var levels = await ListLevels(errorCallback);
            var result = levels?.AsParallel()
                                .Where(levelInfo => levelInfo.Name.Length >= name.Length && levelInfo.Name[..name.Length]
                                                       .Equals(name, StringComparison.OrdinalIgnoreCase));

            if (result != null) {
                return result.ToList();
            }

            errorCallback?.Invoke(null, "Such level does not exist.");
            return null;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while searching level by name. {e.Message}");
            return null;
        }
    }

    public async Task<LevelInfo?> GetLevelByName(string name,
                                                 string? creator = null, 
                                                 EventHandler<string>? errorCallback = null) {
        try {
            var levels = await ListLevels(errorCallback);

            var result = levels?.AsParallel()
                                .Where(levelInfo => 
                                           levelInfo.Name.Length >= name.Length && levelInfo.Name[..name.Length]
                                              .Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (result == null) {
                errorCallback?.Invoke(null, "Such level does not exist");
                return null;
            }

            var filtered = result.ToList();
            if (filtered.Count == 1 || (creator == null && filtered.Count > 0)) {
                return filtered[0];
            }

            if (creator == null) {
                errorCallback?.Invoke(null, "Such level does not exist");
                return null;
            }

            foreach (var level in filtered) {
                if (creator.Equals(level.Creator, StringComparison.InvariantCultureIgnoreCase)) {
                    return level;
                }
            }

            errorCallback?.Invoke(null, "Such level does not exist");
            return null;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while searching level by name. {e.Message}");
            return null;
        }
    }

    public async Task<LevelInfo?> GetLevelByPlacement(int placement, EventHandler<string>? errorCallback = null) {
        try {
            if (placement < 1) {
                errorCallback?.Invoke(null, "Given an invalid placement");
                return null;
            }

            var levels = await ListLevels(errorCallback);

            if (placement <= levels?.Count) {
                return levels[placement - 1];
            }

            errorCallback?.Invoke(null, "Given an invalid placement");
            return null;

        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while searching level by placement. {e.Message}");
            return null;
        }
    }
    
    //public async Task<GListUserProfile>
}