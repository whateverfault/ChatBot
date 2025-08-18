using ChatBot.api.json;
using ChatBot.bot.shared;
using ChatBot.bot.shared.interfaces;

namespace ChatBot.bot.services.translator;

public class TranslatorOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    protected override string Name => "translator";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    public string ProjectId => _saveData!.ProjectId;
    public string Location => _saveData!.Location;
    public string GoogleToken => _saveData!.GoogleToken;
    public string VkToken => _saveData!.VkToken;
    public string TargetLanguage => _saveData!.TargetLanguage;
    public TranslationService TranslationService => _saveData!.TranslationService;
    

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

    public void SetProjectId(string projectId) {
        _saveData!.ProjectId = projectId;
        Save();
    }
    
    public void SetLocation(string location) {
        _saveData!.Location = location;
        Save();
    }
    
    public void SetGoogleToken(string token) {
        _saveData!.GoogleToken = token;
        Save();
    }
    
    public void SetVkToken(string token) {
        _saveData!.VkToken = token;
        Save();
    }

    public void SetTargetLanguage(string language) {
        _saveData!.TargetLanguage = language;
        Save();
    }

    public void SetTranslationService(TranslationService service) {
        _saveData!.TranslationService = service;
        Save();
    }
}