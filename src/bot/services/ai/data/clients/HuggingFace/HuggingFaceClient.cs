using System.Text;
using ChatBot.bot.services.ai.data.clients.DeepSeek;
using ChatBot.bot.services.ai.data.clients.HuggingFace.data;
using ChatBot.bot.services.ai.data.clients.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.ai.data.clients.HuggingFace;

public class HuggingFaceClient : AiClient {
    private readonly HttpClient _httpClient = new HttpClient();


    public override async Task<string?> GetResponse(string prompt, AiChatHistory chatHistory, AiData aiData, EventHandler<string>? callback = null) {
        var messages = new List<DeepSeekMessage> {
                                                     new DeepSeekMessage("system", aiData.BasePrompt),
                                                 };

        foreach (var message in chatHistory.Messages) {
            if (!string.IsNullOrEmpty(message.UserPrompt)) {
                messages.Add(new DeepSeekMessage("user", message.UserPrompt));
            }

            if (!string.IsNullOrEmpty(message.AiResponse)) {
                messages.Add(new DeepSeekMessage("assistant", message.AiResponse));
            }
        }
        
        messages.Add(new DeepSeekMessage("user", prompt));
        
        var requestData = new {
                                  model = $"{aiData.Model}",
                                  messages,
                                  stream = false,
                              };
            
        var json = JsonConvert.SerializeObject(requestData); 
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        try{
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {aiData.ApiKey}");
            
            var response = await _httpClient.PostAsync(aiData.Endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(this,  $"HuggingFace API request failed. Status: {response.StatusCode}. Response: {responseContent}");
                return null;
            }
            
            var message = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseContent);
            
            if (message?.Choices == null || message.Choices.Count == 0) {
                callback?.Invoke(this,  "HuggingFace API request failed.");
                return null;
            }
            
            return message.Choices.Count < 1?
                       null :
                       message.Choices[0].Message == null
                           ? string.Empty
                           : message.Choices[0].Message?.Content;
        }
        catch (Exception e) {
            callback?.Invoke(this, e.ToString());
            return null;
        }
    }
}