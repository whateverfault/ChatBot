using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.moderation;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared;
using ChatBot.bot.shared.handlers;
using SaveData = ChatBot.bot.services.level_requests.data.saved.SaveData;

namespace ChatBot.bot.services.level_requests;

public class LevelRequestsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "level_requests";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public static ModerationService ModerationService => (ModerationService)Services.Get(ServiceId.Moderation);
    
    public override State ServiceState => _saveData!.ServiceState;
    public int PatternIndex => _saveData!.PatternIndex;
    public string RewardId => _saveData!.RewardId;
    public ReqState ReqState => _saveData!.ReqState;
    public Restriction Restriction => _saveData!.Restriction;
    

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