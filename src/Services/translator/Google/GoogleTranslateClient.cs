using System.Text;
using ChatBot.services.logger;
using ChatBot.services.translator.Google.Request;
using ChatBot.services.translator.Google.Response;
using Newtonsoft.Json;

namespace ChatBot.services.translator.Google;

public class GoogleTranslateClient {
    private readonly HttpClient _httpClient;
    private readonly string _projectId;
    private readonly string _location;
    private readonly string _token;


    public GoogleTranslateClient(string projectId, string location, string token) {
        _projectId = projectId;
        _location = location;
        _token = token;
        _httpClient = new HttpClient();
    }
    
    public async Task<DetectLanguageResponse?> DetectLanguage(string text, LoggerService? logger = null) {
        var endpoint = $"https://translate.googleapis.com/v3beta1/projects/{_projectId}/locations/{_location}:detectLanguage";
        var requestBody = new
                          {
                              content = text,
                              mimeType = "text/plain",
                          };
        var jsonBody = JsonConvert.SerializeObject(requestBody);

        try {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                                                         new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            var response = await _httpClient.PostAsync(
                                                        endpoint,
                                                        new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                       );
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Failed to detect language. Status: {response.StatusCode}. Response: {responseContent}");
                return null;
            }
            
            var responseData = 
                JsonConvert.DeserializeObject<DetectLanguageResponse>(responseContent);
            return responseData;
        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while detecting a language. {e.Message}");
            return null;
        }
    }
    
    public async Task<TranslateTextResponse?> Translate(List<string> texts, string targetLanguage, string? sourceLanguage = null, LoggerService? logger = null) {
        var endpoint = $"https://translate.googleapis.com/v3beta1/projects/{_projectId}/locations/{_location}:translateText";

        var request = new TranslateTextRequest
                      {
                          Contents = texts,
                          TargetLanguageCode = targetLanguage,
                          SourceLanguageCode = sourceLanguage,
                      };
        try {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            
            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode) {
                logger?.Log(LogLevel.Error, $"Translation failed: {response.StatusCode}. {responseContent}");
                return null;
            }

            return JsonConvert.DeserializeObject<TranslateTextResponse>(responseContent);
        } catch (Exception e) {
            logger?.Log(LogLevel.Error, $"Error while Translating. {e.Message}");
            return null;
        }
    }
}