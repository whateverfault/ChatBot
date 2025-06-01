using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.message_randomizer;

public enum MessageState {
    NotGuessed,
    Guessed
}

public class MessageRandomizerOptions : Options {
    private SaveData? _saveData;

    protected override string Name => "message_randomizer";
    protected override string OptionsPath => Path.Combine(Directories.serviceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public State LoggerState => _saveData!.LoggerState;
    public int CounterMax => _saveData!.CounterMax;
    public int Counter { get; private set; }

    public State Randomness => _saveData!.Randomness;
    public int RandomValue { get; private set; }

    public Range Spreading => new(_saveData!.SpreadingFrom, _saveData!.SpreadingTo);
    public MessageState MessageState => _saveData!.MessageState;
    public Message LastGeneratedMessage => _saveData!.LastGeneratedMessage;
    public List<Message> Logs => _saveData!.Logs;


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
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.serviceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        const int counterMax = 20;
        _saveData = new SaveData(
                                 counterMax,
                                 State.Disabled,
                                 State.Disabled,
                                 State.Disabled,
                                 1,
                                 counterMax,
                                 MessageState.NotGuessed,
                                 new Message(string.Empty, string.Empty),
                                 []
                                );
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

    public override State GetState() {
        return ServiceState;
    }

    public void SetRandomnessState(State state) {
        _saveData!.Randomness = state;
        Save();
    }

    public void SetLoggerState(State state) {
        _saveData!.LoggerState = state;
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
        Save();
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