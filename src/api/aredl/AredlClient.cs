using System.Net;
using System.Text.RegularExpressions;
using ChatBot.api.aredl.data;
using ChatBot.api.aredl.responses;
using Newtonsoft.Json;

namespace ChatBot.api.aredl;

public class AredlClient {
    private static readonly SocketsHttpHandler _httpHandler = new SocketsHttpHandler
                                                              {
                                                                  PooledConnectionLifetime  = TimeSpan.Zero,
                                                                  PooledConnectionIdleTimeout = TimeSpan.Zero,
                                                                  MaxConnectionsPerServer = 50,
                                                                  UseCookies = false,
                                                                  AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                                                              };
    private static readonly HttpClient _httpClient = new HttpClient(_httpHandler);
    private readonly AredlCache? _cache;
    

    public AredlClient(bool caching) {
        if (caching) {
            _cache = new AredlCache();
        }
    }

    public void ResetCache() {
        _cache?.ResetCache();
    }
    
    public async Task<ListLevelsResponse?> ListLevels(EventHandler<string>? callback = null) {
        try {
            if (_cache is { Levels: not null, }) {
                return _cache.Levels;
            }
            
            var requestMessage = new HttpRequestMessage { 
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new Uri("https://api.aredl.net/v2/api/aredl/levels"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while fetching list levels data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<List<LevelInfo>>(content);
            if (deserialized == null) return null;
                
            var result = new ListLevelsResponse(deserialized);
            _cache?.CacheLevelsList(result); 
            return result;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching list levels data: {e}");
            return null;
        }
    }

    public async Task<List<LevelInfo>?> GetLevelsByName(string name, EventHandler<string>? callback = null) {
        try {
            var levels = await ListLevels(callback);
            var result = levels?.Data.AsParallel()
                                .Where(levelInfo =>
                                           levelInfo.Name.Length >= name.Length && levelInfo.Name[..name.Length]
                                              .Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if (result != null) {
                return result.ToList();
            }

            callback?.Invoke(null, "Such level does not exist");
            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while searching level by name. {e}");
            return null;
        }
    }

    public async Task<LevelInfo?> GetLevelByName(string name, string? creator = null,
                                                        EventHandler<string>? callback = null) {
        try {
            var levels = await ListLevels(callback);

            var result = levels?.Data.AsParallel()
                                .Where(levelInfo =>
                                           levelInfo.Name.Length >= name.Length && levelInfo.Name[..name.Length]
                                              .Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (result == null) {
                callback?.Invoke(null, "Such level does not exist");
                return null;
            }

            var filtered = result.ToList();
            if (filtered.Count == 1 || (creator == null && filtered.Count > 0)) {
                return filtered[0];
            }

            if (creator == null) {
                callback?.Invoke(null, "Such level does not exist");
                return null;
            }

            var creatorRegex = CreatorRegex();

            foreach (var level in filtered) {
                var match = creatorRegex.Match(level.Name);
                if (creator.Equals(match.Value[1..^1], StringComparison.CurrentCultureIgnoreCase)) {
                    return level;
                }
            }

            callback?.Invoke(null, "Such level does not exist");
            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while searching level by name. {e}");
            return null;
        }
    }

    public async Task<LevelInfo?> GetLevelByPlacement(int placement, EventHandler<string>? callback = null) {
        try {
            if (placement < 1) {
                callback?.Invoke(null, "Given an invalid placement");
                return null;
            }

            var levels = await ListLevels(callback);

            if (placement < levels?.Data.Count) {
                return levels.Data[placement - 1];
            }

            callback?.Invoke(null, "Given an invalid placement");
            return null;

        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while searching level by placement. {e}");
            return null;
        }
    }

    public async Task<ListLevelsResponse?> ListPlatformerLevels(EventHandler<string>? callback = null) {
        try {
            if (_cache is { PlatformerLevels: not null, }) {
                return _cache.PlatformerLevels;
            }
            
            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new Uri("https://api.aredl.net/v2/api/arepl/levels"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while fetching list levels data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<List<LevelInfo>>(content);
            if (deserialized == null) return null;
                
            var result = new ListLevelsResponse(deserialized);
            _cache?.CachePlatformerLevelsList(result);
            return result;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching list levels data: {e}");
            return null;
        }
    }

    public async Task<List<LevelInfo>?> GetPlatformerLevelsByName(
        string name, EventHandler<string>? callback = null) {
        try {
            var levels = await ListPlatformerLevels(callback);

            var result = levels?.Data.AsParallel()
                                .Where(levelInfo =>
                                           levelInfo.Name.Length >= name.Length && levelInfo.Name[..name.Length]
                                              .Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if (result != null) {
                return result.ToList();
            }

            callback?.Invoke(null, "Such level does not exist");
            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while searching level by name. {e}");
            return null;
        }
    }

    public async Task<LevelInfo?> GetPlatformerLevelByName(string name, string? creator = null,
                                                                  EventHandler<string>? callback = null) {
        try {
            var levels = await ListPlatformerLevels(callback);

            var result = levels?.Data.AsParallel()
                                .Where(levelInfo =>
                                           levelInfo.Name.Length >= name.Length && levelInfo.Name[..name.Length]
                                              .Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (result == null) {
                callback?.Invoke(null, "Such level does not exist");
                return null;
            }

            var filtered = result.ToList();
            if (filtered.Count == 1 || (creator == null && filtered.Count > 0)) {
                return filtered[0];
            }

            if (creator == null) {
                callback?.Invoke(null, "Such level does not exist");
                return null;
            }

            var creatorRegex = CreatorRegex();

            foreach (var level in filtered) {
                var match = creatorRegex.Match(level.Name);
                if (creator.Equals(match.Value[1..^1], StringComparison.CurrentCultureIgnoreCase)) {
                    return level;
                }
            }

            callback?.Invoke(null, "Such level does not exist");
            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while searching level by name. {e}");
            return null;
        }
    }

    public async Task<LevelInfo?> GetPlatformerLevelByPlacement(int placement,
                                                                       EventHandler<string>? callback = null) {
        try {
            if (placement < 1) {
                callback?.Invoke(null, "Given an invalid placement");
                return null;
            }

            var levels = await ListPlatformerLevels(callback);

            if (placement < levels?.Data.Count) {
                return levels.Data[placement - 1];
            }

            callback?.Invoke(null, "Given an invalid placement");
            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while searching level by placement. {e}");
            return null;
        }
    }

    public async Task<UserProfile?> FindProfile(string username, EventHandler<string>? callback = null) {
        try {
            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new
                                                                    Uri($"https://api.aredl.net/v2/api/aredl/leaderboard?name_filter={username}&page=1"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while fetching user profile data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<LeaderboardResponse>(content);
            if (result?.data?.Count > 0) {
                return result.data[0];
            }

            callback?.Invoke(null,
                             $"Error while fetching user profile data. Status: {response.StatusCode}. Response: {content}");
            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching user profile data: {e}");
            return null;
        }
    }

    public async Task<RecordInfo?> GetRecord(string levelId, string userId,
                                                    EventHandler<string>? callback = null) {
        try {
            var levelRecords = await ListUserRecords(userId, callback);
            if (levelRecords?.records.Count + levelRecords?.verified.Count < 1) {
                callback?.Invoke(null, "Error while fetching user record data.");
                return null;
            }

            foreach (var record in levelRecords?.verified!) {
                if (record.level.id != levelId) continue;
                return record;
            }

            foreach (var record in levelRecords.records) {
                if (record.level.id != levelId) continue;
                return record;
            }

            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching user record data: {e}");
            return null;
        }
    }

    public async Task<UserProfile?> FindPlatformerProfile(string username, EventHandler<string>? callback = null) {
        try {
            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new
                                                                    Uri($"https://api.aredl.net/v2/api/arepl/leaderboard?name_filter={username}&page=1"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while fetching platformer profile data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<LeaderboardResponse?>(content);
            if (result?.data?.Count > 0) {
                return result.data[0];
            }

            callback?.Invoke(null,
                             $"Error while fetching platformer profile data. Status: {response.StatusCode}. Response: {content}");
            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching platformer profile data: {e}");
            return null;
        }
    }
    
    public async Task<RecordInfo?> GetPlatformerRecord(string levelId, string userId,
                                                       EventHandler<string>? callback = null) {
        try {
            var levelRecords = await ListUserPlatformerRecords(userId, callback);
            if (levelRecords == null) return null;
            
            if (levelRecords.records.Count + levelRecords.verified.Count < 1) {
                callback?.Invoke(null, "Error while fetching user record data.");
                return null;
            }

            foreach (var record in levelRecords.verified) {
                if (record.level.id != levelId) continue;
                return record;
            }

            foreach (var record in levelRecords.records) {
                if (record.level.id != levelId) continue;
                return record;
            }

            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching user record data: {e}");
            return null;
        }
    }

    public async Task<ListUserRecordsResponse?> ListUserRecords(string userId,
                                                                EventHandler<string>? callback = null) {
        try {
            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new
                                                                    Uri($"https://api.aredl.net/v2/api/aredl/profile/{userId}"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<ListUserRecordsResponse>(content);
            return result;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching level records data: {e}");
            return null;
        }
    }

    public async Task<ListUserRecordsResponse?> ListUserPlatformerRecords(
        string userId, EventHandler<string>? callback = null) {
        try {
            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new
                                                                    Uri($"https://api.aredl.net/v2/api/arepl/profile/{userId}"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<ListUserRecordsResponse>(content);
            return result;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching level records data: {e}");
            return null;
        }
    }

    public async Task<LevelDetails?> GetLevelDetails(string id, EventHandler<string>? callback = null) {
        try {
            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new
                                                                    Uri($"https://api.aredl.net/v2/api/aredl/levels/{id}"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<LevelDetails>(content);
            return result;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching level records data: {e}");
            return null;
        }
    }

    public async Task<LevelDetails?> GetPlatformerLevelDetails(string id, EventHandler<string>? callback = null) {
        try {
            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new
                                                                    Uri($"https://api.aredl.net/v2/api/arepl/levels/{id}"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<LevelDetails>(content);
            return result;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching level records data: {e}");
            return null;
        }
    }

    public async Task<ListClansResponse?> ListClans(EventHandler<string>? callback = null) {
        try {
            if (_cache is { Clans: not null, }) {
                return _cache.Clans;
            }
            
            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new
                                                                    Uri($"https://api.aredl.net/v2/api/aredl/leaderboard/clans?per_page={int.MaxValue}"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while listing clans info. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<ListClansResponse>(content);
            if (deserialized == null) return null;
            
            if (deserialized.Data.Count >= 1) {
                _cache?.CacheClansList(deserialized);
                return deserialized;
            }

            callback?.Invoke(null,
                             $"Error while fetching clan info. Status: {response.StatusCode}. Response: {content}");
            return null;

        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while listing clans info: {e}");
            return null;
        }
    }

    public async Task<ClanInfo?> GetClan(string tag, EventHandler<string>? callback = null) {
        try {
            var clans = await ListClans(callback);
            if (clans == null) {
                callback?.Invoke(null, "Error while fetching clan info");
                return null;
            }

            var filtered = clans.Data
                                .AsParallel()
                                .Where(clanData => clanData.clan.Tag.Equals(tag,
                                                                            StringComparison.CurrentCultureIgnoreCase));
            var filteredList = filtered.ToList();
            if (filteredList.Count >= 1) {
                return filteredList[0];
            }

            callback?.Invoke(null, "Such clan does not exist");
            return null;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching clan info: {e}");
            return null;
        }
    }

    public async Task<ClanRecordsResponse?> GetClanRecords(string id, EventHandler<string>? callback = null) {
        try {
            var requestMessage = new HttpRequestMessage {
                                                            Method = HttpMethod.Get,
                                                            RequestUri =
                                                                new Uri($"https://api.aredl.net/v2/api/aredl/clan/{id}"),
                                                        };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(null,
                                 $"Error while listing clans info. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<ClanRecordsResponse>(content);
            return result;
        }
        catch (Exception e) {
            callback?.Invoke(null, $"Error while fetching clan info: {e}");
            return null;
        }
    }
    
    private Regex CreatorRegex() {
        return new Regex(@"\(([^)]+)\)");
    }
}