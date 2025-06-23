using ChatBot.bot.interfaces;
using ChatBot.Services.ai.HF;
using ChatBot.Services.ai.Ollama;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.Static;
using ChatBot.shared.interfaces;

namespace ChatBot.Services.ai;

public enum AiMode {
    Local,
    Hf,
}

public class AiService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    private OllamaClient _ollamaClient = null!;
    private HfClient _hfClient = null!;
    
    public override string Name => ServiceName.Ai;
    public override AiOptions Options { get; } = new();


    public async Task<string?> GenerateText(string prompt) {
        var response = string.Empty;

        switch (Options.AiMode) {
            case AiMode.Local: {
                response = await GenerateTextLocal(prompt);
                break;
            }
            case AiMode.Hf: {
                response = await GenerateTextHf(prompt);
                break;
            }
        }
        
        return response;
    } 
    
    private async Task<string?> GenerateTextLocal(string prompt) {
        try {
            return await _ollamaClient.GenerateText(
                                                    model: Options.LocalModel,
                                                    prompt: $"{Options.LocalPrompt} {prompt}"
                                                    );
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"An Error Occured While Local AI Text Generation: {e.Message}");
            return null;
        }
    }

    private async Task<string?> GenerateTextHf(string prompt) {
        try {
            var generated = await _hfClient.GenerateText($"{Options.LocalPrompt} {prompt}");
            if (Options.LocalAiFallback == State.Enabled && string.IsNullOrEmpty(generated)) {
                _logger.Log(LogLevel.Warning, "Activated Local AI Fallback");
                generated = await GenerateTextLocal(prompt);
            }
            return generated;
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"An Error Occured While HF AI Text Generation: {e.Message}");
            return null;
        }
    }
    
    public string GetLocalPrompt() {
        return Options.LocalPrompt;
    }

    public void SetLocalPrompt(string prompt) {
        Options.SetLocalPrompt(prompt);
    }
    
    public string GetHfPrompt() {
        return Options.HfPrompt;
    }

    public void SetHfPrompt(string prompt) {
        Options.SetHfPrompt(prompt);
    }
    
    public string GetModel() {
        return Options.LocalModel;
    }

    public void SetModel(string prompt) {
        Options.SetModel(prompt);
    }
    
    public string GetHfModel() {
        return Options.HfModel;
    }

    public void SetHfModel(string prompt) {
        Options.SetHfModel(prompt);
        _hfClient = new HfClient(Options.HfToken, Options.HfApiUrl, Options.HfModel, _logger);
    }
    
    public string GetHfProvider() {
        return Options.HfProvider;
    }

    public void SetHfProvider(string model) {
        Options.SetHfProvider(model);
        _hfClient = new HfClient(Options.HfToken, Options.HfApiUrl, Options.HfModel, _logger);
    }
    
    public void SetHfToken(string token) {
        _hfClient = new HfClient(Options.HfToken, Options.HfApiUrl, Options.HfModel, _logger);
        Options.SetHfToken(token);
    }

    public string GetHfToken() {
        return Options.HfToken;
    }

    public int GetLocalAiFallbackAsInt() {
        return (int)Options.LocalAiFallback;
    }
    
    public void LocalAiFallbackNext() {
        Options.SetLocalAiFallback((State)(((int)Options.LocalAiFallback+1)%Enum.GetValues(typeof(State)).Length));
    }
    
    public int GetAiModeAsInt() {
        return (int)Options.AiMode;
    }
    
    public void AiModeNext() {
        Options.SetAiMode((AiMode)(((int)Options.AiMode+1)%Enum.GetValues(typeof(AiMode)).Length));
    }
    
    public override void Init(Bot bot) {
        _ollamaClient = new OllamaClient();
        if (!Options.TryLoad()) {
            Options.SetDefaults();
            return;
        }
        _hfClient = new HfClient(Options.HfToken, Options.HfApiUrl, Options.HfModel, _logger);
    }
}