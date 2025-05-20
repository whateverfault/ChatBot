using ChatBot.Shared;
using ChatBot.Shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.message_randomizer;

public class MessageRandomizerOptions : Options {
    private SaveData? _saveData = new();
    private Range Spreading => _saveData!.spreading;
    
    protected override string Name => "message_randomizer";
    protected override string OptionsPath => Path.Combine(Directories.serviceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.state;
    public int CounterMax => _saveData!.counterMax;
    public int Counter => _saveData!.counter;
    public State Randomness => _saveData!.randomness;
    public int RandomValue => _saveData!.randomValue;
    public List<Message> Logs => _saveData!.logs;
    

    public override bool Load() {
        if (!Path.Exists(OptionsPath)) return false;
        JsonUtils.TryRead(OptionsPath, out _saveData);
        return true;
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.serviceDirectory, Name), _saveData);
    }
    
    public void SetDefaults() {
        const int counterMax = 20;
        _saveData = new SaveData(counterMax, State.Enabled,1..counterMax);
    }
    
    public void IncreaseCounter() {
        _saveData!.counter++;
    }

    public void ZeroCounter() {
        _saveData!.counter = 0;
    }

    public void SetServiceState(State state) {
        _saveData!.state = state;
    }

    public void SetRandomnessState(State state) {
        _saveData!.randomness = state;
    }
    
    public void SetSpreading(Range range) {
        _saveData!.spreading = range;
    }

    public void SetRandomValue() {
        _saveData!.randomValue = Random.Shared.Next(Spreading.Start.Value, Spreading.End.Value);
    }
}