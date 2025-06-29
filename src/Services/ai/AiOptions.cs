using ChatBot.Services.ai.AiClients.interfaces;
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
    public List<AiData> AiData => _saveData!.AiData;
    public string GoogleProjectId => _saveData!.GoogleProjectId;
    public AiKind AiKind => _saveData!.AiKind;


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
        _saveData = new SaveData(
                                 [
                                     new AiData(
                                                "Empty",
                                                "Empty",
                                                "Empty",
                                                "http://localhost:11434/api/generate",
                                                "Empty",
                                                new AiFallback(State.Disabled, AiKind.Ollama)
                                                ),
                                     new AiData(
                                                "Empty",
                                                "Empty",
                                                "Empty",
                                                "https://router.huggingface.co/Empty/v1/chat/completions",
                                                "Empty",
                                                new AiFallback(State.Disabled, AiKind.Ollama)
                                               ),
                                     new AiData(
                                                "Empty",
                                                "deepseek-chat",
                                                "Empty",
                                                "https://api.deepseek.com/chat/completions",
                                                "Empty",
                                                new AiFallback(State.Disabled, AiKind.Ollama)
                                               ),
                                     new AiData(
                                                "Empty",
                                                "publishers/google/models/gemini-pro",
                                                "Empty",
                                                "https://aiplatform.googleapis.com/v1/projects/{projectId}/locations/{location}/{model}:generateContent\"",
                                                "Empty",
                                                new AiFallback(State.Disabled, AiKind.Ollama)
                                               ),
                                 ]
                                 );
        Save();
    }
    
    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }
    
    public void SetAiKind(AiKind kind) {
        _saveData!.AiKind = kind;
        Save();
    }

    public void SetGoogleProjectId(string projectId) {
        _saveData!.GoogleProjectId = projectId;
        Save();
    }
}