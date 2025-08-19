using System.Net.Http.Headers;
using System.Text;
using ChatBot.bot.services.translator.Google.Request;
using ChatBot.bot.services.translator.Google.Response;
using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.Google;

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
    
    public async Task<DetectLanguageResponse?> DetectLanguage(string text, EventHandler<string>? callback = null) {
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
                new AuthenticationHeaderValue("Bearer", _token);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                                                         new MediaTypeWithQualityHeaderValue("application/json"));
            
            var response = await _httpClient.PostAsync(
                                                        endpoint,
                                                        new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                       );
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                var deserialized =
                    JsonConvert.DeserializeObject<DetectLanguageResponse>(responseContent);
                return deserialized;
            }

            callback?.Invoke(this , $"Failed to detect language. Status: {response.StatusCode}. Response: {responseContent}");
            return null;
        } catch (Exception e) {
            callback?.Invoke(this , $"Error while detecting a language. {e}");
            return null;
        }
    }
    
    public async Task<TranslateTextResponse?> Translate(List<string> texts, string targetLanguage, string? sourceLanguage = null, EventHandler<string>? callback = null) {
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
                new AuthenticationHeaderValue("Bearer", _token);
            
            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                var deserialized = JsonConvert.DeserializeObject<TranslateTextResponse>(responseContent);
                return deserialized;
            }

            callback?.Invoke(this , $"Translation failed: {response.StatusCode}. {responseContent}");
            return null;
        } catch (Exception e) {
            callback?.Invoke(this , $"Error while Translating. {e}");
            return null;
        }
    }
}