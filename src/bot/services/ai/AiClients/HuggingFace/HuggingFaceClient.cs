using System.Text;
using ChatBot.api.twitch.client;
using ChatBot.bot.services.ai.AiClients.interfaces;
using ChatBot.bot.services.logger;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.AiClients.HuggingFace;

public class HuggingFaceClient : AiClient {
    private readonly HttpClient _httpClient = new HttpClient();


    public override async Task<string?> GetResponse(string prompt, AiData aiData, LoggerService? logger = null) {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {aiData.ApiKey}");
        
        var requestData = new
                          {
                              messages = new[]
                                         {
                                             new
                                             {
                                                 role = "user",
                                                 content = $"{aiData.BasePrompt} {prompt}",
                                             },
                                         },
                              model = aiData.Model,
                              stream = false,
                          };
        var json = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        try {
            var response = await _httpClient.PostAsync(aiData.Endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseContent);
            return message?.Choices?.Count < 1?
                       null :
                       message?.Choices?[0].Message?.Content;
        }
        catch (Exception e) {
            logger?.Log(LogLevel.Error, e.ToString());
            return null;
        }
    }
}