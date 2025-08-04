using ChatBot.services.ai.AiClients.interfaces;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.services.ai;

public class AiOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    protected override string Name => "ai";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    public override State ServiceState => _saveData!.ServiceState;
    public List<AiData> AiData => _saveData!.AiData;
    public string GoogleProjectId => _saveData!.GoogleProjectId;
    public AiKind AiKind => _saveData!.AiKind;

    
    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }
    
    public override void Save() {
        lock (_fileLock) {
            JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }
    
    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
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