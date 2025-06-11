using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.moderation;

public class ModerationOptions : Options {
    private SaveData? _saveData;
    
    protected override string Name => "moderation";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<ModAction> ModerationActions => _saveData!.ModerationActions;
    public List<WarnedUser> WarnedUsers => _saveData!.WarnedUsers;
    
    
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
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        _saveData = new SaveData(
                                 State.Disabled,
                                 [
                                 new ModAction(
                                               0,
                                               600,
                                               "Реквесты только для випов и выше",
                                               2,
                                               Restriction.Vip
                                               ),
                                 new ModAction(
                                               1,
                                               69,
                                               "Эта команда доступна только випам и выше",
                                               3,
                                               Restriction.Vip
                                              ),
                                 ],
                                 []
                                 );
        Save();
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }

    public void AddModAction(ModAction action) {
        ModerationActions.Add(action);
        Save();
    }

    public void RemoveModAction(int index) {
        ModerationActions.RemoveAt(index);
        Save();
    }

    public void AddWarnedUser(WarnedUser warnedUser) {
        WarnedUsers.Add(warnedUser);
        Save();
    }
}