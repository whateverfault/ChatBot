using System.Text;
using ChatBot.Services.logger;
using Newtonsoft.Json;

namespace ChatBot.Services.ai.HF;

public class HfClient {
    private readonly LoggerService? _logger;
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private readonly string _apiUrl;
    private readonly string _model;


    public HfClient(string token, string apiUrl, string model, LoggerService? logger = null) {
        _httpClient = new HttpClient();
        _token = token;
        _apiUrl = apiUrl;
        _model = model;
        _logger = logger;
    }

    public async Task<string?> GenerateText(string prompt) {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
        
        var requestData = new
                          {
                              messages = new[]
                                         {
                                             new
                                             {
                                                 role = "user",
                                                 content = prompt
                                             }
                                         },
                              model = _model,
                              stream = false
                          };
        var json = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        try {
            var response = await _httpClient.PostAsync(_apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseContent);
            _logger?.Log(LogLevel.Info, responseContent);
            return message?.Choices?.Count < 1?
                       null :
                       message?.Choices?[0].Message?.Content;
        }
        catch (Exception e) {
            _logger?.Log(LogLevel.Error, e.Message);
            return null;
        }
    }
}