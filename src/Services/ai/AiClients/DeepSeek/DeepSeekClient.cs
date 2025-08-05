using System.Text;
using ChatBot.services.ai.AiClients.interfaces;
using ChatBot.services.logger;
using Newtonsoft.Json;

namespace ChatBot.services.ai.AiClients.DeepSeek;

public class DeepSeekClient : AiClient {
    private readonly HttpClient _httpClient = new HttpClient();
    
    
    public override async Task<string?> GetResponse(string prompt, AiData aiData, LoggerService? logger = null) {
        var requestBody = new
                          {
                              model = aiData.Model,
                              messages = new[]
                                         {
                                             new {
                                                     role = "system",
                                                     content = aiData.BasePrompt,
                                                 },
                                             new {
                                                     role = "user",
                                                     content = prompt,
                                                 },
                                         },
                              stream = false,
                          };

        var jsonBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        try {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {aiData.ApiKey}");

            var response = await _httpClient.PostAsync(aiData.Endpoint, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error,  $"Status: {response.StatusCode}. Response: {responseJson}");
                return null;
            }
            
            dynamic responseData = JsonConvert.DeserializeObject(responseJson)!;
            return responseData.choices[0].message.content;
        } catch (Exception e) {
            logger?.Log(LogLevel.Error,  $"Caught an Exception. {e}");
            return null;
        }
    }
}