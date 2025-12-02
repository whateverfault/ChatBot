using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.text_generator.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.text_generator;

public class TextGeneratorOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    private static string Name => "text_generator";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    public int ContextSize => _saveData!.ContextSize;
    public int MaxLength => _saveData!.MaxLength;
    public Dictionary<string, Dictionary<string, int>> Model => _saveData!.Model;
    

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

    public void SetContextSize(int contextSize) {
        _saveData!.ContextSize = contextSize;
        Save();
        
    }

    public int GetContextSize() {
        return ContextSize;
    }

    public void SetMaxLength(int maxLength) {
        _saveData!.MaxLength = maxLength;
        Save();
    }

    public int GetMaxLength() {
        return MaxLength;
    }
}