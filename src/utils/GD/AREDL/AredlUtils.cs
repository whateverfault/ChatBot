using System.Text.RegularExpressions;
using ChatBot.Services.logger;
using ChatBot.utils.GD.AREDL.Data;
using ChatBot.utils.GD.AREDL.Responses;
using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL;

public partial class AredlUtils {
    private static readonly HttpClient _httpClient = new();
    
    
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
            logger?.Log(LogLevel.Error, $"Error while fetching list levels data: {e.Message}");
            return null;
        }
    }
    
    public static async Task<LevelInfo?> GetLevelByName(string name, string? creator = null, LoggerService? logger = null) {
        try {
            var levels = await ListLevels(logger);

            var result = levels?.data?.AsParallel()
                             .Where(levelInfo => levelInfo.name.Length >= name.Length && levelInfo.name[0..name.Length].Equals(name, StringComparison.CurrentCultureIgnoreCase));
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
            logger?.Log(LogLevel.Error, $"Error while searching level by name. {e.Message}");
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
            logger?.Log(LogLevel.Error, $"Error while searching level by placement. {e.Message}");
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
            logger?.Log(LogLevel.Error, $"Error while fetching list levels data: {e.Message}");
            return null;
        }
    }
    
    public static async Task<LevelInfo?> GetPlatformerLevelByName(string name, string? creator = null, LoggerService? logger = null) {
        try {
            var levels = await ListPlatformerLevels(logger);

            var result = levels?.data?.AsParallel()
                             .Where(levelInfo => levelInfo.name.Length >= name.Length && levelInfo.name[0..name.Length].Equals(name, StringComparison.CurrentCultureIgnoreCase));
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
            logger?.Log(LogLevel.Error, $"Error while searching level by name. {e.Message}");
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
            logger?.Log(LogLevel.Error, $"Error while searching level by placement. {e.Message}");
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
            if (result?.data.Count > 0) {
                return result.data[0];
            }
            
            logger?.Log(LogLevel.Error, $"Error while fetching user profile data. Status: {response.StatusCode}. Response: {content}");
            return null;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching user profile data: {e.Message}");
            return null;
        }
    }
    
    public static async Task<List<SubmissionInfo?>?> ListRecords(string levelId, LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.aredl.net/v2/api/aredl/levels/{levelId}/records"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<List<SubmissionInfo?>>(content);
            logger?.Log(LogLevel.Info, "Successfully fetched level records data");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching level records data: {e.Message}");
            return null;
        }
    }
    
    public static async Task<SubmissionInfo?> GetRecord(string levelId, string userId, LoggerService? logger = null) {
        try {
            var levelRecords = await ListRecords(levelId, logger);
            if (levelRecords?.Count < 1) {
                logger?.Log(LogLevel.Error, $"Error while fetching user record data.");
                return null;
            }

            foreach (var record in levelRecords!) {
                if (record?.submittedBy?.id != userId) continue;
                return record;
            }
            return null;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching user record data: {e.Message}");
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
            if (result?.data.Count > 0) {
                return result.data[0];
            }
            
            logger?.Log(LogLevel.Error, $"Error while fetching platformer profile data. Status: {response.StatusCode}. Response: {content}");
            return null;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching platformer profile data: {e.Message}");
            return null;
        }
    }
    
    public static async Task<List<SubmissionInfo?>?> ListPlatformerRecords(string levelId, LoggerService? logger = null) {
        try {
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Get,
                                     RequestUri = new Uri($"https://api.aredl.net/v2/api/arepl/levels/{levelId}/records"),
                                 };

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Error while fetching level records data. Status: {response.StatusCode}. Response: {content}");
                return null;
            }
                
            var result = JsonConvert.DeserializeObject<List<SubmissionInfo?>>(content);
            logger?.Log(LogLevel.Info, "Successfully fetched level records data");
            return result;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching level records data: {e.Message}");
            return null;
        }
    }
    
    public static async Task<SubmissionInfo?> GetPlatformerRecord(string levelId, string userId, LoggerService? logger = null) {
        try {
            var levelRecords = await ListPlatformerRecords(levelId, logger);
            if (levelRecords?.Count < 1) {
                logger?.Log(LogLevel.Error, $"Error while fetching user platformer record data.");
                return null;
            }

            foreach (var record in levelRecords!) {
                if (record?.submittedBy?.id != userId) continue;
                return record;
            }
            return null;
        }
        catch(Exception e){
            logger?.Log(LogLevel.Error, $"Error while fetching user platformer record data: {e.Message}");
            return null;
        }
    }
    
    [GeneratedRegex(@"\(([^)]+)\)")]
    private static partial Regex CreatorRegex();
}