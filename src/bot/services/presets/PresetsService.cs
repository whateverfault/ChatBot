using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.presets;

public class PresetsService : Service {
    public override string Name => ServiceName.Presets;
    public override PresetsOptions Options { get; } = new PresetsOptions();

    public event EventHandler<Preset>? OnPresetAdded;
    public event EventHandler<int>? OnPresetRemoved;
    
    
    public void AddPreset(string name) {
        var preset = new Preset(name, Options.GetNextId());
        preset.Create();
        
        Options.AddPreset(preset);
        Options.SetCurrentPreset(Options.Presets.Count-1);
        OnPresetAdded?.Invoke(this, preset);
    }

    public bool RemovePreset(int index) {
        var result =  Options.RemovePreset(index);
        OnPresetRemoved?.Invoke(this, index);
        return result;
    }

    public List<Preset> GetPresets() {
        return Options.Presets;
    }
    
    public bool SetCurrentPreset(int index) {
        if (index < 0 || index >= Options.Presets.Count) return false;
        
        Options.SetCurrentPreset(index);
        Options.Presets[Options.CurrentPreset].Load();
        return true;
    }

    public override void Init() {
        base.Init();

        SetCurrentPreset(Options.CurrentPreset);
    }
}