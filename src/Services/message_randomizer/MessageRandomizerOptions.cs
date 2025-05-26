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

    public override State State => _saveData!.serviceState;
    public State LoggerState => _saveData!.loggerState;
    public int CounterMax => _saveData!.counterMax;
    public int Counter { get; private set; }

    public State Randomness => _saveData!.randomness;
    public int RandomValue { get; private set; }

    public Range Spreading => new(_saveData!.spreadingFrom, _saveData!.spreadingTo);
    public MessageState MessageState => _saveData!.messageState;
    public Message LastGeneratedMessage => _saveData!.lastGeneratedMessage;
    public List<Message> Logs => _saveData!.logs;


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
        _saveData!.serviceState = state;
        Save();
    }

    public override State GetState() {
        return State;
    }

    public void SetRandomnessState(State state) {
        _saveData!.randomness = state;
        Save();
    }

    public void SetLoggerState(State state) {
        _saveData!.loggerState = state;
        Save();
    }

    public void SetCounterMax(int value) {
        _saveData!.counterMax = value;
        Save();
    }

    public void SetCounterMaxDynamic(dynamic value) {
        SetCounterMax(value);
    }

    
    public int GetCounterMax() {
        return CounterMax;
    }

    public dynamic GetCounterMaxDynamic() {
        return CounterMax;
    }
    
    public void SetMessageState(MessageState state) {
        _saveData!.messageState = state;
        Save();
    }

    public MessageState GetMessageState() {
        return _saveData!.messageState;
    }

    public void SetLastGeneratedMessage(Message message) {
        _saveData!.lastGeneratedMessage = message;
        Save();
    }

    public void SetSpreading(Range range) {
        var minRangeStart = 1;
        var maxRangeEnd = CounterMax;

        var start = range.Start.Value;
        var end = range.End.Value;

        if (start < minRangeStart) {
            start = minRangeStart;
        }
        if (end > maxRangeEnd) {
            end = maxRangeEnd;
        }
        if (start > end) {
            (start, end) = (end, start);
        }

        _saveData!.spreadingFrom = start;
        _saveData!.spreadingTo = end;
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