using System.Text;
using ChatBot.bot.services.ai.AiClients.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.AiClients.DeepSeek;

public class DeepSeekClient : AiClient {
    private readonly HttpClient _httpClient = new HttpClient();
    
    
    public override async Task<string?> GetResponse(string prompt, AiData aiData, EventHandler<string>? callback = null) {
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
                callback?.Invoke(this,  $"Status: {response.StatusCode}. Response: {responseJson}");
                return null;
            }
            
            dynamic responseData = JsonConvert.DeserializeObject(responseJson)!;
            return responseData.choices[0].message.content;
        } catch (Exception e) {
            callback?.Invoke(this,  $"Caught an Exception. {e}");
            return null;
        }
    }
}