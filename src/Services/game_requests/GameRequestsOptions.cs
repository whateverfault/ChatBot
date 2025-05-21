using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.game_requests;

public class GameRequestsOptions : Options{
    private SaveData? _saveData;
    
    protected override string Name => "game_requests";
    protected override string OptionsPath => Path.Combine(Directories.serviceDirectory+Name, $"{Name}_opt.json");
    
    public override State State => _saveData!.state;
    public List<GameRequest>? GameRequests => _saveData!.gameRequests;
    public HashSet<int>? GameRequestsSet => _saveData!.gameRequestsSet;
    public Dictionary<string, int>? GameRequestsPoint => _saveData!.gameRequestsPoints;
    
    
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
        _saveData = new SaveData(State.Disabled, [], [], []);
        Save();
    }

    public override void SetState(State state) {
        _saveData!.state = state;
        Save();
    }

    public override State GetState() {
        return State;
    }
}