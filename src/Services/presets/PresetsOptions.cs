using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.presets;

public class PresetsOptions : Options {
    private SaveData? _saveData;
    
    protected override string Name => "presets";
    protected override string OptionsPath => Path.Combine(Directories.ConfigDirectory, $"{Name}.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<Preset> Presets => _saveData!.Presets;
    public int CurrentPreset => _saveData!.CurrentPreset;
    
    
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
        JsonUtils.WriteSafe(OptionsPath, Directories.ConfigDirectory, _saveData);
    }

    public override void SetDefaults() {
        var preset = new Preset("default");
        preset.Create();
        _saveData = new SaveData([preset]);
        Save();
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
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

    public void RemovePreset(int index) {
        Presets.RemoveAt(index);
        Save();
    }
}