using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.ai;

public class AiOptions : Options {
    private SaveData? _saveData;
    
    protected override string Name => "ai";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    public override State ServiceState => _saveData!.ServiceState;
    public string LocalModel => _saveData!.Model;
    public string LocalPrompt => _saveData!.LocalPrompt;
    public string HfModel => _saveData!.HfModel;
    public string HfProvider => _saveData!.HfProvider;
    public string HfPrompt => _saveData!.HfPrompt;
    public string HfToken => _saveData!.HfToken;
    public string HfApiUrl => $"https://router.huggingface.co/{HfProvider}/v1/chat/completions";
    public State LocalAiFallback => _saveData!.LocalAiFallback;
    public AiMode AiMode => _saveData!.AiMode;


    public override bool TryLoad() {
        return JsonUtils.TryRead(OptionsPath, out _saveData);
    }

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            SetDefaults();
        }
    }
    
    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }
    
    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }
    
    public void SetLocalPrompt(string prompt) {
        _saveData!.LocalPrompt = prompt;
        Save();
    }
    
    public void SetHfPrompt(string prompt) {
        _saveData!.HfPrompt = prompt;
        Save();
    }
    
    public void SetModel(string model) {
        _saveData!.Model = model;
        Save();
    }

    public void SetHfModel(string model) {
        _saveData!.HfModel = model;
        Save();
    }
    
    public void SetHfProvider(string provider) {
        _saveData!.HfProvider = provider;
        Save();
    }
    
    public void SetHfToken(string token) {
        _saveData!.HfToken = token;
        Save();
    }
    
    public void SetLocalAiFallback(State fallback) {
        _saveData!.LocalAiFallback = fallback;
        Save();
    }
    
    public void SetAiMode(AiMode mode) {
        _saveData!.AiMode = mode;
        Save();
    }
}