using ChatBot.api.twitch.client;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.ai.AiClients.DeepSeek;
using ChatBot.bot.services.ai.AiClients.Google;
using ChatBot.bot.services.ai.AiClients.HuggingFace;
using ChatBot.bot.services.ai.AiClients.interfaces;
using ChatBot.bot.services.ai.AiClients.Ollama;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.ai;

public enum AiKind {
    Ollama,
    HuggingFace,
    DeepSeek,
    VertexAi,
}

public class AiService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private readonly List<AiClient> _aiClients = [];
    
    public override string Name => ServiceName.Ai;
    public override AiOptions Options { get; } = new AiOptions();


    public async Task<string?> GetResponse(string prompt) {
        var aiIndex = (int)Options.AiKind;
        var aiClient = _aiClients[aiIndex];
        var aiData = Options.AiData[aiIndex];
        
        var response = 
            await aiClient.GetResponse(prompt, aiData, (_, message) => {
                                                           _logger.Log(LogLevel.Error, message);
                                                       });
        if (response != null || aiData.Fallback.FallbackState != State.Enabled) {
            return response;
        }

        aiIndex = (int)aiData.Fallback.FallbackAi;
        aiClient = _aiClients[aiIndex];
        aiData = Options.AiData[aiIndex];
        
        response = await aiClient.GetResponse(prompt, aiData, (_, message) => {
                                                                  _logger.Log(LogLevel.Error, message);
                                                              });
        return response;
    }
    
    #region Ollama
    
    public string GetOllamaPrompt() {
        return Options.AiData[(int)AiKind.Ollama].BasePrompt;
    }

    public void SetOllamaPrompt(string prompt) {
        Options.AiData[(int)AiKind.Ollama].BasePrompt = prompt;
        Options.Save();
    }
    
    public string GetOllamaModel() {
        return Options.AiData[(int)AiKind.Ollama].Model;
    }

    public void SetOllamaModel(string model) {
        Options.AiData[(int)AiKind.Ollama].Model = model;
        Options.Save();
    }
    #endregion
    #region HuggingFace
    
    public string GetHfPrompt() {
        return Options.AiData[(int)AiKind.HuggingFace].BasePrompt;
    }

    public void SetHfPrompt(string prompt) {
        Options.AiData[(int)AiKind.HuggingFace].BasePrompt = prompt;
        Options.Save();
    }
    
    public string GetHfModel() {
        return Options.AiData[(int)AiKind.HuggingFace].Model;
    }

    public void SetHfModel(string model) {
        Options.AiData[(int)AiKind.HuggingFace].Model = model;
        Options.Save();
    }
    
    public void SetHfProvider(string provider) {
        var aiIndex = (int)AiKind.HuggingFace;
        
        Options.AiData[aiIndex].Provider = provider;
        Options.AiData[aiIndex].Endpoint = $"https://router.huggingface.co/{provider}/v1/chat/completions";
        Options.Save();
    }
    
    public string GetHfProvider() {
        return Options.AiData[(int)AiKind.HuggingFace].Provider;
    }
    
    public void SetHfToken(string apiKey) {
        Options.AiData[(int)AiKind.HuggingFace].ApiKey = apiKey;
        Options.Save();
    }

    public string GetHfToken() {
        return Options.AiData[(int)AiKind.HuggingFace].ApiKey;
    }
    
    public void HfFallbackStateNext() {
        var aiIndex = (int)AiKind.HuggingFace;
        var fallbackAi = (int)Options.AiData[aiIndex].Fallback.FallbackState;
        
        Options.AiData[aiIndex].Fallback.FallbackState =
            (State)((fallbackAi+1)%Enum.GetValues(typeof(State)).Length);
        Options.Save();
    }

    public int GetHfFallbackStateAsInt() {
        return (int)Options.AiData[(int)AiKind.HuggingFace].Fallback.FallbackState;
    }
    
    public void HfFallbackAiNext() {
        Options.AiData[(int)AiKind.HuggingFace].Fallback.FallbackAi =
            (AiKind)(((int)Options.AiData[(int)AiKind.HuggingFace].Fallback.FallbackAi+1)%Enum.GetValues(typeof(AiKind)).Length);
        Options.Save();
    }
    
    public int GetHfFallbackAiAsInt() {
        return (int)Options.AiData[(int)AiKind.HuggingFace].Fallback.FallbackAi;
    }
    #endregion
    #region DeepSeek
    
    public string GetDeepSeekPrompt() {
        return Options.AiData[(int)AiKind.DeepSeek].BasePrompt;
    }

    public void SetDeepSeekPrompt(string prompt) {
        Options.AiData[(int)AiKind.DeepSeek].BasePrompt = prompt;
        Options.Save();
    }
    
    public string GetDeepSeekModel() {
        return Options.AiData[(int)AiKind.DeepSeek].Model;
    }

    public void SetDeepSeekModel(string model) {
        Options.AiData[(int)AiKind.DeepSeek].Model = model;
        Options.Save();
    }
    
    public void SetDeepSeekToken(string apiKey) {
        Options.AiData[(int)AiKind.DeepSeek].ApiKey = apiKey;
        Options.Save();
    }

    public string GetDeepSeekToken() {
        return Options.AiData[(int)AiKind.DeepSeek].ApiKey;
    }

    public int GetDeepSeekFallbackStateAsInt() {
        return (int)Options.AiData[(int)AiKind.DeepSeek].Fallback.FallbackState;
    }
    
    public void DeepSeekFallbackStateNext() {
        var aiIndex = (int)AiKind.DeepSeek;
        var fallbackAi = (int)Options.AiData[aiIndex].Fallback.FallbackState;
        
        Options.AiData[aiIndex].Fallback.FallbackState =
            (State)((fallbackAi+1)%Enum.GetValues(typeof(State)).Length);
        Options.Save();
    }
    
    public int GetDeepSeekFallbackAiAsInt() {
        return (int)Options.AiData[(int)AiKind.DeepSeek].Fallback.FallbackAi;
    }
    
    public void DeepSeekFallbackAiNext() {
        var aiIndex = (int)AiKind.DeepSeek;
        var fallbackAi = (int)Options.AiData[aiIndex].Fallback.FallbackAi;
        
        Options.AiData[aiIndex].Fallback.FallbackAi =
            (AiKind)((fallbackAi+1)%Enum.GetValues(typeof(AiKind)).Length);
        Options.Save();
    }
    
    #endregion
    #region Vertex

    public string GetVertexPrompt() {
        return Options.AiData[(int)AiKind.VertexAi].BasePrompt;
    }

    public void SetVertexPrompt(string prompt) {
        Options.AiData[(int)AiKind.VertexAi].BasePrompt = prompt;
        Options.Save();
    }
    
    public string GetVertexModel() {
        return Options.AiData[(int)AiKind.VertexAi].Model;
    }

    public void SetVertexModel(string model) {
        Options.AiData[(int)AiKind.VertexAi].Model = model;
        
        Options.AiData[(int)AiKind.VertexAi].Endpoint = $"https://aiplatform.googleapis.com/v1/projects/{Options.GoogleProjectId}/locations/general/{model}:generateContent";
        Options.Save();
    }

    public string GetGoogleProjectId() {
        return Options.GoogleProjectId;
    }

    public void SetGoogleProjectId(string projectId) {
        var model = Options.AiData[(int)AiKind.VertexAi].Model;
        Options.AiData[(int)AiKind.VertexAi].Endpoint = $"https://aiplatform.googleapis.com/v1/projects/{projectId}/locations/general/{model}:generateContent";
        Options.SetGoogleProjectId(projectId);
    }
    
    public void SetVertexToken(string apiKey) {
        Options.AiData[(int)AiKind.VertexAi].ApiKey = apiKey;
        Options.Save();
    }

    public string GetVertexToken() {
        return Options.AiData[(int)AiKind.VertexAi].ApiKey;
    }

    public int GetVertexFallbackStateAsInt() {
        return (int)Options.AiData[(int)AiKind.VertexAi].Fallback.FallbackState;
    }
    
    public void VertexFallbackStateNext() {
        var aiIndex = (int)AiKind.VertexAi;
        var fallbackAi = (int)Options.AiData[aiIndex].Fallback.FallbackState;
        
        Options.AiData[aiIndex].Fallback.FallbackState =
            (State)((fallbackAi+1)%Enum.GetValues(typeof(State)).Length);
        Options.Save();
    }
    
    public int GetVertexFallbackAiAsInt() {
        return (int)Options.AiData[(int)AiKind.VertexAi].Fallback.FallbackAi;
    }
    
    public void VertexFallbackAiNext() {
        var aiIndex = (int)AiKind.VertexAi;
        var fallbackAi = (int)Options.AiData[aiIndex].Fallback.FallbackAi;
        
        Options.AiData[aiIndex].Fallback.FallbackAi =
            (AiKind)((fallbackAi+1)%Enum.GetValues(typeof(AiKind)).Length);
        Options.Save();
    }

    #endregion
    
    public int GetAiKindAsInt() {
        return (int)Options.AiKind;
    }
    
    public void AiKindNext() {
        Options.SetAiKind((AiKind)(((int)Options.AiKind+1)%Enum.GetValues(typeof(AiKind)).Length));
    }
    
    public override void Init() {
        base.Init();
        
        _aiClients.Add(new OllamaClient());
        _aiClients.Add(new HuggingFaceClient());
        _aiClients.Add(new DeepSeekClient());
        _aiClients.Add(new VertexAiClient());
    }
}