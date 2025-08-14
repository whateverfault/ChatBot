using ChatBot.bot.services.chat_logs;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared;
using ChatBot.bot.shared.interfaces;
using ChatBot.bot.utils;

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

    public Range Spreading => new Range(_saveData!.SpreadingFrom, _saveData!.SpreadingTo);
    public MessageState MessageState => _saveData!.MessageState;
    public Message LastGeneratedMessage => _saveData!.LastGeneratedMessage;
    public ChatLogsService ChatLogsService => (ChatLogsService)ServiceManager.GetService(ServiceName.ChatLogs);
    
    
    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
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

        var start = range.Start.Value;
        var end = range.End.Value;

        if (start < minRangeStart) {
            start = minRangeStart;
        }
        if (start > end) {
            (start, end) = (end, start);
        }

        _saveData!.SpreadingFrom = start;
        _saveData!.SpreadingTo = end;
        SetRandomValue();
    }
    
    public Range GetSpreading() {
        return Spreading;
    }
    
    public void SetRandomValue() {
        RandomValue = Random.Shared.Next(Spreading.Start.Value, Spreading.End.Value);
        Save();
    }

    public int GetRandomValue() {
        return RandomValue;
    }
}