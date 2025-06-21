using System.Text;
using Newtonsoft.Json;
using ChatBot.Services.logger;

namespace ChatBot.Services.ai.Ollama;

public class OllamaClient {
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public OllamaClient(string baseUrl = "http://localhost:11434") {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task<string?> GenerateText(string model, string prompt, Action<string?>? streamCallback = null, LoggerService? logger = null) {
        try {
            var requestData = new {
                                      model,
                                      prompt,
                                      stream = streamCallback != null
                                  };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/generate", content);

            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Ollama API request failed with status code: {response.StatusCode}");
                return null;
            }

            if (streamCallback != null) {
                await using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                while (!reader.EndOfStream) {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) continue;

                    var jsonResponse = JsonConvert.DeserializeObject<OllamaResponse>(line);
                    if (jsonResponse != null && !string.IsNullOrEmpty(jsonResponse.Response)) {
                        streamCallback.Invoke(jsonResponse.Response);
                    }
                }
                return string.Empty;
            }
            var responseJson = await response.Content.ReadAsStringAsync();
            var ollamaResponse = JsonConvert.DeserializeObject<OllamaResponse>(responseJson);
            return ollamaResponse?.Response ?? string.Empty;
        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while an ai was text-generating: {e.Message} ");
            return null;
        }
    }
}