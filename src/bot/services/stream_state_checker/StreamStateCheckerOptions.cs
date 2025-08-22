using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.stream_state_checker.Data;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.stream_state_checker;

public class StreamStateCheckerOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    protected override string Name => "stream_state";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public StreamState StreamState => _saveData!.StreamState;
    public StreamStateMeta StreamStateMeta => _saveData!.StreamStateMeta;
    

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

    public void SetCheckCooldown(long cooldown) {
        StreamStateMeta.CheckCooldown = cooldown;
        Save();
    }

    public void SetLastCheckedTime() {
        StreamStateMeta.LastChecked = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Save();
    }
    
    public void AddOfflineTime() {
        if (IsOnline() || StreamStateMeta.LastChecked == 0) {
            SetOffline();
            return;
        }
        
        StreamState.OfflineTime += StreamStateMeta.CheckCooldown;
        Save();
    }
    
    public void AddOnlineTime() {
        if (!IsOnline()) {
            SetOnline();
            return;
        }
        
        StreamState.OnlineTime += StreamStateMeta.CheckCooldown;
        StreamState.LastOnline = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Save();
    }

    public void SetOffline() {
        if (!StreamState.WasOnline) return;
        
        StreamState.OfflineTime = 0;
        StreamState.OnlineTime = 0;
        StreamState.WasOnline = false;
        Save();
    }
    
    public void SetOnline() {
        if (StreamState.WasOnline) return;
        
        StreamState.OfflineTime = 0;
        StreamState.OnlineTime = 0;
        StreamState.WasOnline = true;
        StreamState.LastOnline = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Save();
    }

    public bool IsOnline() {
        return StreamState.WasOnline;
    }
    
    public long GetLastOnline() {
        return StreamState.LastOnline;
    }
    
    public long GetOnlineTime() {
        return StreamState.OnlineTime;
    }
    
    public long GetOfflineTime() {
        return StreamState.OfflineTime;
    }

    public long GetCheckCooldown() {
        return StreamStateMeta.CheckCooldown;
    }

    public long GetLastCheckTime() {
        return StreamStateMeta.LastChecked;
    }
}