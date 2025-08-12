using ChatBot.shared;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.bot.services.presets;

public class PresetsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    protected override string Name => "presets";
    protected override string OptionsPath => Path.Combine(Directories.ConfigDirectory, $"{Name}.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<Preset> Presets => _saveData!.Presets;
    public int CurrentPreset => _saveData!.CurrentPreset;
    

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            JsonUtils.WriteSafe(OptionsPath, Directories.ConfigDirectory, _saveData);
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

    public void SetCurrentPreset(int index) {
        _saveData!.CurrentPreset = index;
        Save();
    }

    public int GetCurrentPreset() {
        return CurrentPreset;
    }

    public void AddPreset(Preset preset) {
        Presets.Add(preset);
        Save();
    }

    public bool RemovePreset(int index) {
        if (index < 0 || index >= Presets.Count) {
            return false;
        }
        
        Presets.RemoveAt(index);
        Save();
        return true;
    }
}