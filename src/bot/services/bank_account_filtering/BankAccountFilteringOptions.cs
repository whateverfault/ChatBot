using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.bank_account_filtering.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.bank_account_filtering;

public class BankAccountFilteringOptions : Options {
    private readonly object _fileLock = new object();

    private SaveData? _saveData;

    private static string Name => "bank_account_filtering";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory + Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    
    private long StreamsPassed => _saveData!.StreamsPassed;
    private long PassedStreamsThreshold => _saveData!.PassedStreamsThreshold;
    private long MinStreamLength => _saveData!.MinStreamLength;
    private long LastActiveThreshold => _saveData!.LastActiveThreshold;
    

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

    public void SetLastOnlineThreshold(long value) {
        _saveData!.LastActiveThreshold = value;
        Save();
    }

    public bool LastActiveThresholdInitialized() {
        return LastActiveThreshold != 0;
    }

    public bool StreamsThresholdBreached() {
        return StreamsPassed >= PassedStreamsThreshold;
    }

    public void IncrementPassedStreams() {
        ++_saveData!.StreamsPassed;
        Save();
    }

    public void ZeroPassedStreams() {
        _saveData!.StreamsPassed = 0;
        Save();
    }

    public void SetMinStreamLength(long value) {
        _saveData!.MinStreamLength = value;
        Save();
    }
    
    public long GetMinStreamLength() {
        return MinStreamLength;
    }

    public void SetLastActiveThreshold(long value) {
        _saveData!.LastActiveThreshold = value;
        Save();
    }
    
    public long GetLastActiveThreshold() {
        return LastActiveThreshold;
    }

    public void SetPassedStreamsThreshold(long value) {
        _saveData!.PassedStreamsThreshold = value;
        Save();
    }
    
    public long GetPassedStreamsThreshold() {
        return PassedStreamsThreshold;
    }

    public void SetStreamFiltered(bool value) {
        _saveData!.StreamFiltered = value;
        Save();
    }

    public bool GetStreamFiltered() {
        return _saveData!.StreamFiltered;
    }

    public long GetLastOnline() {
        return _saveData!.LastOnline;
    }

    public void SetLastOnline(long value) {
        _saveData!.LastOnline = value;
        Save();
    }

    public long GetStreamsTimeGapThreshold() {
        return _saveData!.StreamsTimeGapThreshold;
    }
}