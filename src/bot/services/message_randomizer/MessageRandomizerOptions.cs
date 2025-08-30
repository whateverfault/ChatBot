using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_logs;
using ChatBot.bot.shared;
using Range = ChatBot.api.basic.Range;

namespace ChatBot.bot.services.message_randomizer;

public enum MessageState {
    NotGuessed,
    Guessed,
}

public class MessageRandomizerOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    protected override string Name => "message_randomizer";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public int CounterMax => _saveData!.CounterMax;
    public int Counter { get; private set; }

    public State Randomness => _saveData!.Randomness;
    public int RandomValue { get; private set; }

    public Range Spreading => _saveData!.Spreading;
    public MessageState MessageState => _saveData!.MessageState;
    public Message LastGeneratedMessage => _saveData!.LastGeneratedMessage;
    
    
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


    public void IncreaseCounter() {
        Counter++;
    }

    public void ZeroCounter() {
        Counter = 0;
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public void SetRandomnessState(State state) {
        _saveData!.Randomness = state;
        Save();
    }

    public void SetCounterMax(int value) {
        _saveData!.CounterMax = value;
        Save();
    }
    
    public int GetCounterMax() {
        return CounterMax;
    }
    
    public void SetMessageState(MessageState state) {
        _saveData!.MessageState = state;
        Save();
    }

    public MessageState GetMessageState() {
        return _saveData!.MessageState;
    }

    public void SetLastGeneratedMessage(Message message) {
        _saveData!.LastGeneratedMessage = message;
        Save();
    }

    public void SetSpreading(Range range) {
        const int minRangeStart = 1;

        var start = range.Start;
        var end = range.End;

        if (start < minRangeStart) {
            start = minRangeStart;
        }
        if (start > end) {
            (start, end) = (end, start);
        }

        _saveData!.Spreading = new Range(start, end);
        SetRandomValue();
    }
    
    public Range GetSpreading() {
        return Spreading;
    }
    
    public void SetRandomValue() {
        RandomValue = Random.Shared.Next(Spreading.Start, Spreading.End);
        Save();
    }

    public int GetRandomValue() {
        return RandomValue;
    }
}