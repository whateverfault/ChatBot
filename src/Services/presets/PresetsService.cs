using ChatBot.bot.interfaces;
using ChatBot.CLI.CliNodes.Directories;
using ChatBot.Services.interfaces;
using ChatBot.Services.Static;

namespace ChatBot.Services.presets;

public class PresetsService : Service {
    private bot.ChatBot _bot = null!;
    
    public override string Name => ServiceName.Presets;
    public override PresetsOptions Options { get; } = new();


    public void AddPreset(string name, bool hasComment, string comment) {
        var preset = new Preset(name);
        preset.Create();
        
        Options.AddPreset(preset);
        Options.SetCurrentPreset(Options.Presets.Count-1);
    }

    public void RemovePreset(int index) {
        Options.RemovePreset(index);
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
        Options.Presets[Options.CurrentPreset].Load(_bot);
        return true;
    }

    public override void Init(Bot bot) {
        _bot = (bot.ChatBot)bot;
        base.Init(bot);
        Options.Presets[Options.CurrentPreset].Load(bot);
    }
}