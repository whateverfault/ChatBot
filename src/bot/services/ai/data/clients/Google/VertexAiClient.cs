using System.Net.Http.Headers;
using System.Text;
using ChatBot.bot.services.ai.data.clients.Google.Response;
using ChatBot.bot.services.ai.data.clients.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.Google;

public class VertexAiClient : AiClient {
    private static readonly HttpClient _httpClient = new HttpClient();
    

    public override async Task<string?> GetResponse(string prompt, AiChatHistory chatHistory, AiData aiData, EventHandler<string>? callback = null) {
        var messages = new List<VertexAiMessage> {
                                                     new VertexAiMessage("user", [aiData.BasePrompt,]),
                                                 };

        foreach (var message in chatHistory.Messages) {
            messages.Add(new VertexAiMessage("user", [message.UserPrompt,]));
            messages.Add(new VertexAiMessage("model", [message.AiResponse,]));
        }
        
        messages.Add(new VertexAiMessage("user", [prompt,]));
        
        var requestData = new
                          {
                              contents = messages,
                              generationConfig = new
                                                 {
                                                     temperature = 0.7, 
                                                     topP = 0.8,        
                                                     maxOutputTokens = 2048,
                                                 },
                          };
    
        var json = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
    
        try {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", aiData.ApiKey);
            
            var response = await _httpClient.PostAsync(aiData.Endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
        
            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(this , $"API request failed: {response.StatusCode} - {responseContent}");
                return null;
            }
        
            var vertexResponse = JsonConvert.DeserializeObject<VertexAiResponse>(responseContent);
            return vertexResponse?.Candidates?.FirstOrDefault()?.Content?.Parts.FirstOrDefault()?.Text;
        }
        catch (Exception e) {
            callback?.Invoke(this , e.Message);
            return null;
        }
    }
}