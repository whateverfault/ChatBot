using System.Net.Http.Headers;
using System.Text;
using ChatBot.api.twitch.client;
using ChatBot.bot.services.ai.AiClients.Google.Response;
using ChatBot.bot.services.ai.AiClients.interfaces;
using ChatBot.bot.services.logger;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.AiClients.Google;

public class VertexAiClient : AiClient {
    private static readonly HttpClient _httpClient = new HttpClient();
    

    public override async Task<string?> GetResponse(string prompt, AiData aiData, LoggerService? logger = null) {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", aiData.ApiKey);
        
        var requestData = new
                          {
                              contents = new[]
                                         {
                                             new
                                             {
                                                 role = "user",
                                                 parts = new[]
                                                         {
                                                             new
                                                             {
                                                                 text = $"[SYSTEM INSTRUCTIONS]\n{aiData.BasePrompt}\n\n[USER PROMPT]\n{prompt}",
                                                             },
                                                         },
                                             },
                                         },
                              generationConfig = new
                                                 {
                                                     temperature = 0.9, 
                                                     topP = 0.8,        
                                                     maxOutputTokens = 2048,
                                                 },
                          };
    
        var json = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
    
        try {
            var response = await _httpClient.PostAsync(aiData.Endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
        
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"API request failed: {response.StatusCode} - {responseContent}");
                return null;
            }
        
            var vertexResponse = JsonConvert.DeserializeObject<VertexAiResponse>(responseContent);
            return vertexResponse?.Candidates?.FirstOrDefault()?.Content?.Parts.FirstOrDefault()?.Text;
        }
        catch (Exception e) {
            logger?.Log(LogLevel.Error, e.ToString());
            return null;
        }
    }
}