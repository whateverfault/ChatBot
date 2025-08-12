using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.cli.CliNodes.Directories;

namespace ChatBot.bot.services.presets;

public class PresetsService : Service {
    public override string Name => ServiceName.Presets;
    public override PresetsOptions Options { get; } = new PresetsOptions();


    public void AddPreset(string name, bool hasComment, string comment) {
        var preset = new Preset(name);
        preset.Create();
        
        Options.AddPreset(preset);
        Options.SetCurrentPreset(Options.Presets.Count-1);
    }

    public bool RemovePreset(int index) {
        return Options.RemovePreset(index);
    }

    public List<Content> GetPresetsAsContent() {
        return Options
           .Presets
           .Select(preset => new Content(preset.Name, false, ""))
           .ToList();
    }

    public Preset? GetPreset(int index) {
        if (index < 0 || index >= Options.Presets.Count) return null;
        
        return Options.Presets[index];
    }

    public bool SetCurrentPreset(int index) {
        if (index < 0 || index >= Options.Presets.Count) return false;
        
        Options.SetCurrentPreset(index);
        Options.Presets[Options.CurrentPreset].Load();
        return true;
    }

    public override void Init() {
        base.Init();
        
        Options.Presets[Options.CurrentPreset].Load();
    }
}