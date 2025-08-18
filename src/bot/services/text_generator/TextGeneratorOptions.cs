using ChatBot.api.json;
using ChatBot.bot.shared;
using ChatBot.bot.shared.interfaces;

namespace ChatBot.bot.services.text_generator;

public class TextGeneratorOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    protected override string Name => "text_generator";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
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
        _saveData = new SaveData(
                                    State.Disabled,
                                    2,
                                    50,
                                    []
                                );
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
    
    public void Train(string text) {
        var words = text.Split([' ', '\n', '\r', '\t',], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length <= ContextSize) return;

        for (var i = 0; i < words.Length - ContextSize; i++) {
            var context = string.Join(" ", words.Skip(i).Take(ContextSize));
            var nextWord = words[i + ContextSize];

            if (!Model.ContainsKey(context)) {
                Model[context] = new Dictionary<string, int>();
            }

            if (!Model[context].ContainsKey(nextWord)) {
                Model[context][nextWord] = 0;
            }

            Model[context][nextWord]++;
        }
        
        Save();
    }
}