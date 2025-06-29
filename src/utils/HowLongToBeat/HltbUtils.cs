using System.Text;
using ChatBot.Services.logger;
using ChatBot.utils.HowLongToBeat.Request;
using ChatBot.utils.HowLongToBeat.Request.Data;
using ChatBot.utils.HowLongToBeat.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChatBot.utils.HowLongToBeat;

public static class HltbUtils {
    private static readonly HttpClient _httpClient = new(); 
    
    
    public static async Task<FetchGameInfoResponse?> FetchGameInfo(GameFilter filter, LoggerService logger) {
        const string url = "https://howlongtobeat.com/api/seek/d4b2e330db04dbf3";
        
        var request = new SearchRequest
                      {
                          SearchType = "games",
                          SearchTerms = [filter.GameName],
                          SearchPage = 1,
                          Size = 20,
                          UseCache = true,
                          SearchOptions = new SearchOptions
                                          {
                                              Games = new GameSearchOptions
                                                      {
                                                          UserId = 0,
                                                          Platform = filter.Platform,
                                                          SortCategory = "popular",
                                                          RangeCategory = "main",
                                                          RangeTime = filter.RangeTime,
                                                          Gameplay = new Gameplay
                                                                     {
                                                                         Perspective = "",
                                                                         Flow = "",
                                                                         Genre = "",
                                                                         Difficulty = ""
                                                                     },
                                                          RangeYear = filter.RangeYear,
                                                          Modifier = ""
                                                      },
                                              Users = new UserSearchOptions { SortCategory = "postcount" },
                                              Lists = new ListSearchOptions { SortCategory = "follows" },
                                              Filter = "",
                                              Sort = 0,
                                              Randomizer = 0
                                          }
                      };
        
        var jsonSettings = new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore,
                               ContractResolver = new CamelCasePropertyNamesContractResolver()
                           };

        var requestBody = JsonConvert.SerializeObject(request, Formatting.Indented, jsonSettings);
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "YourApp/1.0");
        _httpClient.DefaultRequestHeaders.Referrer = new Uri("https://howlongtobeat.com/");

        try {
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                logger.Log(LogLevel.Error, $"Failed to call an API. Status: {response.StatusCode}. Content: {responseContent}");
                return null;
            }
            
            var deserialized = JsonConvert.DeserializeObject<FetchGameInfoResponse>(responseContent);
            return deserialized;
        }
        catch (Exception ex) {
            logger.Log( LogLevel.Error,$"Exception: {ex.Message}");
            return null;
        }
    }
}