using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.presets.data;
using ChatBot.bot.services.presets.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.presets;

public class PresetsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "presets";
    private static string OptionsPath => Path.Combine(Directories.ConfigDirectory, $"{Name}.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<Preset> Presets => _saveData!.Presets;
    public int CurrentPreset => _saveData!.CurrentPreset;
    

    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }

        ValidatePresets();
    }

    public override void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Directories.ConfigDirectory, _saveData);
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

    public int GetNextId() {
        var max = -1;
        foreach (var preset in Presets) {
            if (preset.Id > max) max = preset.Id;
        }

        return ++max;
    }

    private void ValidatePresets() {
        var lastId = -1;
        var invalid = false;
        
        foreach (var preset in Presets) {
            if (preset.Id <= lastId) {
                invalid = true;
                preset.ReassignId(lastId+1);
            }
            lastId = preset.Id;
        }
        
        if (invalid) Save();
    }
}