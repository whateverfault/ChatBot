using ChatBot.Services.moderation;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.level_requests;

public class LevelRequestsOptions : Options {
    private SaveData? _saveData;
    protected override string Name => "level_requests";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    public override State ServiceState => _saveData!.ServiceState;
    public int PatternIndex => _saveData!.PatternIndex;
    public string RewardId => _saveData!.RewardId;
    public ReqState ReqState => _saveData!.ReqState;
    public Restriction Restriction => _saveData!.Restriction;
    public ModerationService ModerationService { get; set; } = null!;

    
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
        _saveData = new SaveData();
        Save();
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }
    
    public int GetPatternIndex() {
        return PatternIndex;
    }

    public void SetPatternIndex(int index) {
        _saveData!.PatternIndex = index;
        Save();
    }
    
    public Restriction GetRestriction() {
        return Restriction;
    }

    public void SetRestriction(Restriction restriction) {
        _saveData!.Restriction = restriction;
        Save();
    }

    public void SetReqState(ReqState state) {
        _saveData!.ReqState = state;
        Save();
    }

    public void SetRewardId(string rewardId) {
        _saveData!.RewardId = rewardId;
        Save();
    }
}