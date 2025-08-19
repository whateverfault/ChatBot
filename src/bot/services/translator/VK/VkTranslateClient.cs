using System.Net.Http.Headers;
using System.Text;
using ChatBot.bot.services.translator.VK.Response;
using Newtonsoft.Json;

namespace ChatBot.bot.services.translator.VK;

public class VkTranslateClient {
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;


    public VkTranslateClient(string apiKey) {
        _httpClient = new HttpClient();
        _apiKey = apiKey;
    }

    public async Task<string[]?> Translate(string text, string lang, string? sourceLang = null, EventHandler<string>? callback = null) {
        try {
            var endpoint = string.IsNullOrEmpty(sourceLang) ?
                               $"https://api.vk.ru/method/translations.translate?texts={text}&translation_language={lang}&v=5.131" :
                               $"https://api.vk.ru/method/translations.translate?texts={text}&translation_language={lang}&source_lang={sourceLang}&v=5.131";
            
            var requestData = new
                              {
                                  texts = text[0],
                                  translation_language = lang,
                              };
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _apiKey);
            
            var jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                callback?.Invoke(this, $"Failed to make an API call. Status: {response.StatusCode}. Content: {responseContent}");
                return null;
            }
            
            var responseData = 
                JsonConvert.DeserializeObject<VkTranslateResponse>(responseContent);

            if (responseData?.Response != null) {
                return responseData.Response.Text;
            }

            callback?.Invoke(this, "API returned a bad response.");
            return null;
        } catch (Exception e) {
            callback?.Invoke(this, $"Exception: {e}"); 
            return null;
        }
    }
}