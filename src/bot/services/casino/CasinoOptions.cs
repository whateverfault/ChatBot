using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.casino;

public class CasinoOptions : Options{
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    protected override string Name => "casino";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    
    public float RandomValue => _saveData!.RandomValue;
    
    
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
        NewRandomValue();
        Save();
    }
    
    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }
    
    public void NewRandomValue() {
        _saveData!.RandomValue = Random.Shared.NextSingle();
        Save();
    }
}