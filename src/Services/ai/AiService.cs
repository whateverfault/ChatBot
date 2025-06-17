using ChatBot.bot.interfaces;
using ChatBot.Services.ai.Ollama;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.Static;

namespace ChatBot.Services.ai;

public class AiService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private OllamaClient _ollamaClient = null!;
    
    public override string Name => ServiceName.AI;
    public override AiOptions Options { get; } = new();


    public async Task<string?> GenerateText(string prompt) {
        try {
            return await _ollamaClient.GenerateText(
                                                    model: Options.Model,
                                                    prompt: $"{Options.BasePrompt} {prompt}"
                                                    );
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"An Error Occured While AI Text Generation: {e.Message}");
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
    
    public override void Init(Bot bot) {
        _ollamaClient = new OllamaClient();
        base.Init(bot);
    }
}