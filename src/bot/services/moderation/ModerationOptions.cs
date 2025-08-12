using ChatBot.shared;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.bot.services.moderation;

public class ModerationOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    protected override string Name => "moderation";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<ModAction> ModerationActions => _saveData!.ModerationActions;
    public List<WarnedUser> WarnedUsers => _saveData!.WarnedUsers;
    

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

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public void AddModAction(ModAction action) {
        ModerationActions.Add(action);
        Save();
    }

    public bool RemoveModAction(int index) {
        if (index < 0 | index >= ModerationActions.Count) {
            return false;
        }
        
        ModerationActions.RemoveAt(index);
        Save();
        return true;
    }

    public void AddWarnedUser(WarnedUser warnedUser) {
        WarnedUsers.Add(warnedUser);
        Save();
    }

    public bool RemoveWarnedUser(int index) {
        if (index < 0 || index >= WarnedUsers.Count) {
            return false;
        }

        WarnedUsers.RemoveAt(index);
        Save();
        return true;
    }
}