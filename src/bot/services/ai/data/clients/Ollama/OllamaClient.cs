using System.Text;
using ChatBot.bot.services.ai.data.clients.DeepSeek;
using ChatBot.bot.services.ai.data.clients.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.Ollama;

public class OllamaClient : AiClient {
    private readonly HttpClient _httpClient = new HttpClient();


    public override async Task<string?> GetResponse(string prompt, AiChatHistory chatHistory, AiData aiData, EventHandler<string>? callback = null) {
        try {
            var messages = new List<DeepSeekMessage> {
                                                         new DeepSeekMessage("system", "aiData.BasePrompt"),
                                                     };

            foreach (var message in chatHistory.Messages) {
                messages.Add(new DeepSeekMessage("user", message.UserPrompt));
                messages.Add(new DeepSeekMessage("assistant", message.AiResponse));
            }
            
            var requestData = new {
                                      aiData.Model,
                                      messages,
                                      stream = false,
                                  };

            messages.Add(new DeepSeekMessage("user", prompt));
            
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(aiData.Endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(this , $"Ollama request failed. Status: {response.StatusCode}. Response: {responseContent}");
                return null;
            }
            
            var ollamaResponse = JsonConvert.DeserializeObject<OllamaResponse>(responseContent);
            return ollamaResponse?.Response ?? string.Empty;
        } catch (Exception e) {
            callback?.Invoke(this , $"Error while getting ollama response: {e.Data} ");
            return null;
        }
    }
}