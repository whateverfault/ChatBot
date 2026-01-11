using System.Net;
using System.Text.RegularExpressions;
using ChatBot.api.aredl.data;
using ChatBot.api.aredl.responses;
using Newtonsoft.Json;

namespace ChatBot.api.aredl;

public class AredlClient {
    private static readonly SocketsHttpHandler _httpHandler = new SocketsHttpHandler
                                                              {
                                                                  PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                                                                  PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                                                                  MaxConnectionsPerServer = 50,
                                                                  UseCookies = false,
                                                                  AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                                                              };
    private readonly HttpClient _httpClient;
    private readonly AredlCache? _cache;
    

    public AredlClient(bool caching, HttpClient? client = null) {
        _httpClient = client ?? new HttpClient(_httpHandler);
        
        if (caching) {
            _cache = new AredlCache();
        }
    }

    public async Task<int> GetLevelsCount() {
        try {
            await ListLevels();
            return _cache?.Levels?.Data.Count ?? 0;
        }
        catch {
            return 0;
        }
    }
    
    public async Task<int> GetPlatformerLevelsCount() {
        try {
            await ListPlatformerLevels();
            return _cache?.PlatformerLevels?.Data.Count ?? 0;
        }
        catch {
            return 0;
        }
    }
    
    public void ResetCache() {
        _cache?.ResetCache();
    }
    
    public async Task<ListProfilesResponse?> ListProfiles(string? nameFilter = null, EventHandler<string>? errorCallback = null) {
        try {
            var formatedNameFilter = string.IsNullOrEmpty(nameFilter)? 
                                            string.Empty :
                                            $"?name_filter={nameFilter}" ;
            
            using var requestMessage = 
                new HttpRequestMessage(HttpMethod.Get, $"https://api.aredl.net/v2/api/aredl/leaderboard{formatedNameFilter}");
            
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                 $"Error while fetching user profiles. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<ListProfilesResponse>(content);

            return deserialized;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching user profiles: {e.Message}");
            return null;
        }
    }
    
    public async Task<ListLevelsResponse?> ListLevels(EventHandler<string>? errorCallback = null) {
        try {
            if (_cache is { Levels: not null, }) {
                return _cache.Levels;
            }
            
            using var requestMessage = 
                new HttpRequestMessage(HttpMethod.Get, "https://api.aredl.net/v2/api/aredl/levels?exclude_legacy=true");
            
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
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
            errorCallback?.Invoke(null, $"Error while fetching list levels data: {e.Message}");
            return null;
        }
    }

    public async Task<List<LevelInfo>?> GetLevelsByName(string name, EventHandler<string>? errorCallback = null) {
        try {
            var levels = await ListLevels(errorCallback);
            var result = levels?.Data.AsParallel()
                                .Where(levelInfo =>
                                           levelInfo.Name.Length >= name.Length && levelInfo.Name[..name.Length]
                                              .Equals(name, StringComparison.CurrentCultureIgnoreCase));

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

    public async Task<LevelInfo?> GetLevelByName(string name, string? creator = null,
                                                        EventHandler<string>? errorCallback = null) {
        try {
            var levels = await ListLevels(errorCallback);

            var result = levels?.Data.AsParallel()
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

            var creatorRegex = CreatorRegex();

            foreach (var level in filtered) {
                var match = creatorRegex.Match(level.Name);
                if (creator.Equals(match.Value[1..^1], StringComparison.CurrentCultureIgnoreCase)) {
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

            if (placement <= levels?.Data.Count) {
                return levels.Data[placement - 1];
            }

            errorCallback?.Invoke(null, "Given an invalid placement");
            return null;

        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while searching level by placement. {e.Message}");
            return null;
        }
    }

    public async Task<ListLevelsResponse?> ListPlatformerLevels(EventHandler<string>? errorCallback = null) {
        try {
            if (_cache is { PlatformerLevels: not null, }) {
                return _cache.PlatformerLevels;
            }

            using var requestMessage = 
                new HttpRequestMessage(HttpMethod.Get, "https://api.aredl.net/v2/api/arepl/levels");

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
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
            errorCallback?.Invoke(null, $"Error while fetching list levels data: {e.Message}");
            return null;
        }
    }

    public async Task<List<LevelInfo>?> GetPlatformerLevelsByName(
        string name, EventHandler<string>? errorCallback = null) {
        try {
            var levels = await ListPlatformerLevels(errorCallback);

            var result = levels?.Data.AsParallel()
                                .Where(levelInfo =>
                                           levelInfo.Name.Length >= name.Length && levelInfo.Name[..name.Length]
                                              .Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if (result != null) {
                return result.ToList();
            }

            errorCallback?.Invoke(null, "Such level does not exist");
            return null;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while searching level by name. {e.Message}");
            return null;
        }
    }

    public async Task<LevelInfo?> GetPlatformerLevelByName(string name, string? creator = null,
                                                                  EventHandler<string>? errorCallback = null) {
        try {
            var levels = await ListPlatformerLevels(errorCallback);

            var result = levels?.Data.AsParallel()
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

            var creatorRegex = CreatorRegex();

            foreach (var level in filtered) {
                var match = creatorRegex.Match(level.Name);
                if (creator.Equals(match.Value[1..^1], StringComparison.CurrentCultureIgnoreCase)) {
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

    public async Task<LevelInfo?> GetPlatformerLevelByPlacement(int placement,
                                                                       EventHandler<string>? errorCallback = null) {
        try {
            if (placement < 1) {
                errorCallback?.Invoke(null, "Given an invalid placement");
                return null;
            }

            var levels = await ListPlatformerLevels(errorCallback);

            if (placement < levels?.Data.Count) {
                return levels.Data[placement - 1];
            }

            errorCallback?.Invoke(null, "Given an invalid placement");
            return null;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while searching level by placement. {e.Message}");
            return null;
        }
    }

    public async Task<RecordInfo?> GetRecord(string levelId, string userId,
                                                    EventHandler<string>? errorCallback = null) {
        try {
            var levelRecords = await ListUserRecords(userId, errorCallback);
            if (levelRecords?.Records.Count + levelRecords?.Verified.Count < 1) {
                errorCallback?.Invoke(null, "Error while fetching user record data.");
                return null;
            }

            foreach (var record in levelRecords?.Verified!) {
                if (record.Level.Id != levelId) continue;
                return record;
            }

            foreach (var record in levelRecords.Records) {
                if (record.Level.Id != levelId) continue;
                return record;
            }

            return null;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching user record data: {e.Message}");
            return null;
        }
    }

    public async Task<ListProfilesResponse?> ListPlatformerProfiles(string username, EventHandler<string>? errorCallback = null) {
        try {
            using var requestMessage = 
                new HttpRequestMessage(HttpMethod.Get, $"https://api.aredl.net/v2/api/arepl/leaderboard?name_filter={username}");

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                 $"Error while fetching platformer profile data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<ListProfilesResponse?>(content);
            
            return deserialized;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching platformer profile data: {e.Message}");
            return null;
        }
    }
    
    public async Task<RecordInfo?> GetPlatformerRecord(string levelId, string userId,
                                                       EventHandler<string>? errorCallback = null) {
        try {
            var levelRecords = await ListUserPlatformerRecords(userId, errorCallback);
            if (levelRecords == null) return null;
            
            if (levelRecords.Records.Count + levelRecords.Verified.Count < 1) {
                errorCallback?.Invoke(null, "Error while fetching user record data.");
                return null;
            }

            foreach (var record in levelRecords.Verified) {
                if (record.Level.Id != levelId) continue;
                return record;
            }

            foreach (var record in levelRecords.Records) {
                if (record.Level.Id != levelId) continue;
                return record;
            }

            return null;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching user record data: {e.Message}");
            return null;
        }
    }

    public async Task<ListUserRecordsResponse?> ListUserRecords(string userId,
                                                                EventHandler<string>? errorCallback = null) {
        try {
            var requestMessage = 
                new HttpRequestMessage(HttpMethod.Get, $"https://api.aredl.net/v2/api/aredl/profile/{userId}");

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                 $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<ListUserRecordsResponse>(content);
            return result;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching level records data: {e.Message}");
            return null;
        }
    }

    public async Task<ListUserRecordsResponse?> ListUserPlatformerRecords(
        string userId, EventHandler<string>? errorCallback = null) {
        try {
            using var requestMessage = 
                new HttpRequestMessage(HttpMethod.Get, $"https://api.aredl.net/v2/api/arepl/profile/{userId}");

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                 $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<ListUserRecordsResponse>(content);
            return result;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching level records data: {e.Message}");
            return null;
        }
    }

    public async Task<LevelDetails?> GetLevelDetails(string id, EventHandler<string>? errorCallback = null) {
        try {
            using var requestMessage = 
                new HttpRequestMessage(HttpMethod.Get, $"https://api.aredl.net/v2/api/aredl/levels/{id}");

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                 $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<LevelDetails>(content);
            return result;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching level records data: {e.Message}");
            return null;
        }
    }

    public async Task<LevelDetails?> GetPlatformerLevelDetails(string id, EventHandler<string>? errorCallback = null) {
        try {
            using var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, $"https://api.aredl.net/v2/api/arepl/levels/{id}");

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                 $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<LevelDetails>(content);
            return result;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching level records data: {e.Message}");
            return null;
        }
    }

    public async Task<ListClansResponse?> ListClans(EventHandler<string>? errorCallback = null) {
        try {
            if (_cache is { Clans: not null, }) {
                return _cache.Clans;
            }
            
            using var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, $"https://api.aredl.net/v2/api/aredl/leaderboard/clans?per_page={int.MaxValue}");

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                 $"Error while listing clans info. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var deserialized = JsonConvert.DeserializeObject<ListClansResponse>(content);
            if (deserialized == null) return null;
            
            if (deserialized.Data.Count >= 1) {
                _cache?.CacheClansList(deserialized);
                return deserialized;
            }

            errorCallback?.Invoke(null,
                             $"Error while fetching clan info. Status: {response.StatusCode}. Response: {content}");
            return null;

        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while listing clans info: {e.Message}");
            return null;
        }
    }

    public async Task<ClanInfo?> GetClan(string tag, EventHandler<string>? errorCallback = null) {
        try {
            var clans = await ListClans(errorCallback);
            if (clans == null) {
                errorCallback?.Invoke(null, "Error while fetching clan info");
                return null;
            }

            var filtered = clans.Data
                                .AsParallel()
                                .Where(clanData => clanData.Clan.Tag.Equals(tag,
                                                                            StringComparison.CurrentCultureIgnoreCase));
            var filteredList = filtered.ToList();
            if (filteredList.Count >= 1) {
                return filteredList[0];
            }

            errorCallback?.Invoke(null, "Such clan does not exist");
            return null;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching clan info: {e.Message}");
            return null;
        }
    }

    public async Task<ClanRecordsResponse?> GetClanRecords(string id, EventHandler<string>? errorCallback = null) {
        try {
            using var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, $"https://api.aredl.net/v2/api/aredl/clan/{id}");

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                errorCallback?.Invoke(null,
                                 $"Error while listing clans info. Status: {response.StatusCode}. Response: {content}");
                return null;
            }

            var result = JsonConvert.DeserializeObject<ClanRecordsResponse>(content);
            return result;
        }
        catch (Exception e) {
            errorCallback?.Invoke(null, $"Error while fetching clan info: {e.Message}");
            return null;
        }
    }
    
    private Regex CreatorRegex() {
        return new Regex(@"\(([^)]+)\)");
    }
}