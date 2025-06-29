using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.Static;
using ChatBot.Services.translator.Google;
using ChatBot.Services.translator.Google.Data;

namespace ChatBot.Services.translator;

public class TranslatorService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private GoogleTranslateClient _gTranslateClient = null!;
    
    public override string Name => ServiceName.Translator;
    public override TranslatorOptions Options { get; } = new();


    public async Task<string?> Translate(string text, string? targetLang = null) {
        try {
            var targetLanguage = string.IsNullOrEmpty(targetLang) ? Options.TargetLanguage : targetLang;
            var response = await _gTranslateClient.TranslateTextAsync([text], targetLanguage, logger: _logger);
            
            if (response is { Translations.Count: > 0 }) {
                return response.Translations[0].TranslatedText;
            }
            
            _logger.Log(LogLevel.Error, $"Failed to translate '{text}' to {targetLanguage}");
            return null;
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Error while translating. {e.Message}");
            return null;
        }
    }
    
    public async Task<DetectedLanguage?> DetectLanguage(string text) {
        try {
            var response = await _gTranslateClient.DetectLanguage(text, _logger);
            
            if (response is { Languages.Count: > 0 }) {
                return response.Languages[0];
            }
            
            _logger.Log(LogLevel.Error, $"Failed to detect language of '{text}'");
            return null;
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Error while detecting a language. {e.Message}");
            return null;
        }
    }

    public string GetProjectId() {
        return Options.ProjectId;
    }

    public void SetProjectId(string projectId) {
        Options.SetProjectId(projectId);
        _gTranslateClient = new GoogleTranslateClient(Options.ProjectId, Options.Location, Options.Token);
    }

    public string GetLocation() {
        return Options.Location;
    }

    public void SetLocation(string location) {
        Options.SetLocation(location);
        _gTranslateClient = new GoogleTranslateClient(Options.ProjectId, Options.Location, Options.Token);
    }

    public string GetToken() {
        return Options.Token;
    }

    public void SetToken(string token) {
        Options.SetToken(token);
        _gTranslateClient = new GoogleTranslateClient(Options.ProjectId, Options.Location, Options.Token);
    }

    public string GetTargetLanguage() {
        return Options.TargetLanguage;
    }

    public void SetTargetLanguage(string language) {
        Options.SetTargetLanguage(language);
    }
    
    public override void Init(Bot bot) {
        base.Init(bot);
        _gTranslateClient = new GoogleTranslateClient(Options.ProjectId, Options.Location, Options.Token);
    }
}