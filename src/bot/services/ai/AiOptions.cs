using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.ai.AiClients.interfaces;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.ai;

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
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }
    
    public override void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
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

    public void SetCasinoIntegrationState(State state) {
        _saveData!.CasinoIntegration = state;
        Save();
    }
}