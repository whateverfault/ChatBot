using ChatBot.bot.interfaces;
using ChatBot.Services.ai.HF;
using ChatBot.Services.ai.Ollama;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.Static;

namespace ChatBot.Services.ai;

public enum AiMode {
    Local,
    Hf,
}

public class AiService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    private OllamaClient _ollamaClient = null!;
    private HfClient _hfClient = null!;
    
    public override string Name => ServiceName.AI;
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
                                                    model: Options.Model,
                                                    prompt: $"{Options.BasePrompt} {prompt}"
                                                    );
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"An Error Occured While Local AI Text Generation: {e.Message}");
            return null;
        }
    }

    private async Task<string?> GenerateTextHf(string prompt) {
        try {
            return await _hfClient.GenerateText($"{Options.BasePrompt} {prompt}");
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"An Error Occured While HF AI Text Generation: {e.Message}");
            return null;
        }
    }
    
    public string GetBasePrompt() {
        return Options.BasePrompt;
    }

    public void SetBasePrompt(string prompt) {
        Options.SetBasePrompt(prompt);
    }
    
    public string GetModel() {
        return Options.Model;
    }

    public void SetModel(string prompt) {
        Options.SetModel(prompt);
    }
    
    public void SetHfToken(string token) {
        _hfClient = new HfClient(Options.HfToken, Options.HfApiUrl, Options.Model);
        Options.SetHfToken(token);
    }

    public string GetHfToken() {
        return Options.HfToken;
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
        _hfClient = new HfClient(Options.HfToken, Options.HfApiUrl, Options.Model);
    }
}