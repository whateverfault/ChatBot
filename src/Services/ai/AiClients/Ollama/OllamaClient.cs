using System.Text;
using ChatBot.services.ai.AiClients.interfaces;
using ChatBot.services.logger;
using Newtonsoft.Json;

namespace ChatBot.services.ai.AiClients.Ollama;

public class OllamaClient : AiClient {
    private readonly HttpClient _httpClient = new HttpClient();


    public override async Task<string?> GetResponse(string prompt, AiData aiData, LoggerService? logger = null) {
        try {
            var requestData = new {
                                      aiData.Model,
                                      prompt = $"{aiData.BasePrompt} {prompt}",
                                      stream = false,
                                  };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(aiData.Endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Ollama request failed. Status: {response.StatusCode}. Response: {responseContent}");
                return null;
            }
            
            var ollamaResponse = JsonConvert.DeserializeObject<OllamaResponse>(responseContent);
            return ollamaResponse?.Response ?? string.Empty;
        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while getting ollama response: {e} ");
            return null;
        }
    }
}