using System.Text.RegularExpressions;
using ChatBot.api.client;
using ChatBot.bot.services.logger;
using ChatBot.bot.utils.GD.AREDL.Data;
using ChatBot.bot.utils.GD.AREDL.Responses;
using Newtonsoft.Json;

namespace ChatBot.bot.utils.GD.AREDL;

public partial class AredlUtils {
    private static readonly HttpClient _httpClient = new HttpClient();
    
    
    public static async Task<ListLevelsResponse?> ListLevels(LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri("https://api.aredl.net/v2/api/aredl/levels"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching list levels data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var deserialized = JsonConvert.DeserializeObject<List<LevelInfo>>(content);
            var result = new ListLevelsResponse(deserialized);
            logger?.Log(LogLevel.Info, "Successfully fetched list levels data");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching list levels data: {e}");
            return null;
        }
    }
    
    public static async Task<List<LevelInfo>?> GetLevelsByName(string name, LoggerService? logger = null) {
        try {
            var levels = await ListLevels(logger);
            var result = levels?.data?.AsParallel()
                             .Where(levelInfo => levelInfo.name.Length >= name.Length && levelInfo.name[..name.Length].Equals(name, StringComparison.CurrentCultureIgnoreCase));
            
            if (result != null) return result.ToList();
            logger?.Log(LogLevel.Error, "Such level does not exist");
            return null;

        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while searching level by name. {e}");
            return null;
        }
    }
    
    public static async Task<LevelInfo?> GetLevelByName(string name, string? creator = null, LoggerService? logger = null) {
        try {
            var levels = await ListLevels(logger);

            var result = levels?.data?.AsParallel()
                             .Where(levelInfo => levelInfo.name.Length >= name.Length && levelInfo.name[..name.Length].Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (result == null) {
                logger?.Log(LogLevel.Error, "Such level does not exist");
                return null;
            }
            
            var filtered = result.ToList();
            if (filtered.Count == 1 || (creator == null && filtered.Count > 0)) {
                return filtered[0];
            }

            if (creator == null) {
                logger?.Log(LogLevel.Error, "Such level does not exist");
                return null;
            }

            var creatorRegex = CreatorRegex();
            
            foreach (var level in filtered) {
                var match = creatorRegex.Match(level.name);
                if (creator.Equals(match.Value[1..^1], StringComparison.CurrentCultureIgnoreCase)) {
                    return level;
                }
            }
            
            logger?.Log(LogLevel.Error, "Such level does not exist");
            return null;
        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while searching level by name. {e}");
            return null;
        }
    }
    
    public static async Task<LevelInfo?> GetLevelByPlacement(int placement, LoggerService? logger = null) {
        try {
            if (placement < 1) {
                logger?.Log(LogLevel.Error, "Given an invalid placement");
                return null;
            }
            
            var levels = await ListLevels(logger);

            if (placement < levels?.data?.Count) {
                return levels.data?[placement-1];
            }
            
            logger?.Log(LogLevel.Error, "Given an invalid placement");
            return null;

        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while searching level by placement. {e}");
            return null;
        }
    }
    
    public static async Task<ListLevelsResponse?> ListPlatformerLevels(LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri("https://api.aredl.net/v2/api/arepl/levels"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching list levels data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var deserialized = JsonConvert.DeserializeObject<List<LevelInfo>>(content);
            var result = new ListLevelsResponse(deserialized);
            logger?.Log(LogLevel.Info, "Successfully fetched list levels data");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching list levels data: {e}");
            return null;
        }
    }
    
    public static async Task<List<LevelInfo>?> GetPlatformerLevelsByName(string name, LoggerService? logger = null) {
        try {
            var levels = await ListPlatformerLevels(logger);

            var result = levels?.data?.AsParallel()
                             .Where(levelInfo => levelInfo.name.Length >= name.Length && levelInfo.name[..name.Length].Equals(name, StringComparison.CurrentCultureIgnoreCase));
            
            if (result != null) return result.ToList();
            logger?.Log(LogLevel.Error, "Such level does not exist");
            return null;
        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while searching level by name. {e}");
            return null;
        }
    }
    
    public static async Task<LevelInfo?> GetPlatformerLevelByName(string name, string? creator = null, LoggerService? logger = null) {
        try {
            var levels = await ListPlatformerLevels(logger);

            var result = levels?.data?.AsParallel()
                             .Where(levelInfo => levelInfo.name.Length >= name.Length && levelInfo.name[..name.Length].Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (result == null) {
                logger?.Log(LogLevel.Error, "Such level does not exist");
                return null;
            }
            
            var filtered = result.ToList();
            if (filtered.Count == 1 || (creator == null && filtered.Count > 0)) {
                return filtered[0];
            }

            if (creator == null) {
                logger?.Log(LogLevel.Error, "Such level does not exist");
                return null;
            }

            var creatorRegex = CreatorRegex();
            
            foreach (var level in filtered) {
                var match = creatorRegex.Match(level.name);
                if (creator.Equals(match.Value[1..^1], StringComparison.CurrentCultureIgnoreCase)) {
                    return level;
                }
            }
            
            logger?.Log(LogLevel.Error, "Such level does not exist");
            return null;
        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while searching level by name. {e}");
            return null;
        }
    }
    
    public static async Task<LevelInfo?> GetPlatformerLevelByPlacement(int placement, LoggerService? logger = null) {
        try {
            if (placement < 1) {
                logger?.Log(LogLevel.Error, "Given an invalid placement");
                return null;
            }
            
            var levels = await ListPlatformerLevels(logger);

            if (placement < levels?.data?.Count) {
                return levels.data?[placement-1];
            }
            
            logger?.Log(LogLevel.Error, "Given an invalid placement");
            return null;

        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while searching level by placement. {e}");
            return null;
        }
    }

    public static async Task<UserProfile?> FindProfile(string username, LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.aredl.net/v2/api/aredl/leaderboard?name_filter={username}&page=1"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching user profile data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<LeaderboardResponse>(content);
            logger?.Log(LogLevel.Info, "Successfully fetched user profile data");
            if (result?.data?.Count > 0) {
                return result.data[0];
            }
            
            logger?.Log(LogLevel.Error, $"Error while fetching user profile data. Status: {response.StatusCode}. Response: {content}");
            return null;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching user profile data: {e}");
            return null;
        }
    }
    
    public static async Task<RecordInfo?> GetRecord(string levelId, string userId, LoggerService? logger = null) {
        try {
            var levelRecords = await ListUserRecords(userId, logger);
            if (levelRecords?.records.Count+levelRecords?.verified.Count < 1) {
                logger?.Log(LogLevel.Error, "Error while fetching user record data.");
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
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching user record data: {e}");
            return null;
        }
    }
    
    public static async Task<UserProfile?> FindPlatformerProfile(string username, LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.aredl.net/v2/api/arepl/leaderboard?name_filter={username}&page=1"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching platformer profile data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<LeaderboardResponse?>(content);
            logger?.Log(LogLevel.Info, "Successfully fetched platformer profile data");
            if (result?.data?.Count > 0) {
                return result.data[0];
            }
            
            logger?.Log(LogLevel.Error, $"Error while fetching platformer profile data. Status: {response.StatusCode}. Response: {content}");
            return null;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching platformer profile data: {e}");
            return null;
        }
    }
    
    public static async Task<RecordInfo?> GetPlatformerRecord(string levelId, string userId, LoggerService? logger = null) {
        try {
            var levelRecords = await ListUserPlatformerRecords(userId, logger);
            if (levelRecords?.records.Count+levelRecords?.verified.Count < 1) {
                logger?.Log(LogLevel.Error, "Error while fetching user platformer record data.");
                return null;
            }

            foreach (var record in levelRecords?.verified!) {
                if (record.level.id != levelId) continue;
                return record;
            }
            return levelRecords.records
                            .FirstOrDefault(record => record.level.id == levelId);
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching user platformer record data: {e}");
            return null;
        }
    }

    public static async Task<ListUserRecordsResponse?> ListUserRecords(string userId, LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.aredl.net/v2/api/aredl/profile/{userId}"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<ListUserRecordsResponse>(content);
            logger?.Log(LogLevel.Info, "Successfully fetched level records data");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching level records data: {e}");
            return null;
        }
    }
    
    public static async Task<ListUserRecordsResponse?> ListUserPlatformerRecords(string userId, LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.aredl.net/v2/api/arepl/profile/{userId}"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<ListUserRecordsResponse>(content);
            logger?.Log(LogLevel.Info, "Successfully fetched level records data");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching level records data: {e}");
            return null;
        }
    }

    public static async Task<LevelDetails?> GetLevelDetails(string id, LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.aredl.net/v2/api/aredl/levels/{id}"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<LevelDetails>(content);
            logger?.Log(LogLevel.Info, "Successfully fetched level records data");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching level records data: {e}");
            return null;
        }
    }
    
    public static async Task<LevelDetails?> GetPlatformerLevelDetails(string id, LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.aredl.net/v2/api/arepl/levels/{id}"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<LevelDetails>(content);
            logger?.Log(LogLevel.Info, "Successfully fetched level records data");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching level records data: {e}");
            return null;
        }
    }
    
    public static async Task<GetClanResponse?> ListClans(LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri("https://api.aredl.net/v2/api/aredl/leaderboard/clans"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while listing clans info. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<GetClanResponse>(content);
            if (result?.data.Count < 1) {
                logger?.Log(LogLevel.Error, $"Error while fetching clan info. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
            logger?.Log(LogLevel.Info, "Successfully listing clans info");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while listing clans info: {e}");
            return null;
        }
    }
    
    public static async Task<ClanInfo?> GetClan(string tag, LoggerService? logger = null) {
        try {
            var clans = await ListClans(logger);
            if (clans == null) {
                logger?.Log(LogLevel.Error, "Error while fetching clan info");
                return null;
            }

            var filtered = clans.data
                             .AsParallel()
                             .Where(clanData => clanData != null && clanData.clan.tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase));
            var filteredList = filtered.ToList();
            if (filteredList.Count < 1) {
                logger?.Log(LogLevel.Error, "Such clan does not exist");
                return null;
            }
            logger?.Log(LogLevel.Info, "Successfully listing clans info");
            return filteredList[0];
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching clan info: {e}");
            return null;
        }
    }
    
    public static async Task<ClanRecordsResponse?> GetClanRecords(string id, LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.aredl.net/v2/api/aredl/clan/{id}"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while listing clans info. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<ClanRecordsResponse>(content);
            logger?.Log(LogLevel.Info, "Successfully listing clans info");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching clan info: {e}");
            return null;
        }
    }
    
    [GeneratedRegex(@"\(([^)]+)\)")]
    private static partial Regex CreatorRegex();
}