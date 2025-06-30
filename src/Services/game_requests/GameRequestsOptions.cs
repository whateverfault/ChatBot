using ChatBot.Services.game_requests.Data;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.game_requests;

public class GameRequestsOptions : Options {
    private SaveData? _saveData;

    protected override string Name => "game_requests";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<GameRequest> GameRequests => _saveData!.GameRequests;
    public GameRequestsFilter GameRequestsFilter => _saveData!.GameRequestsFilter;
    public List<string> GameRequestsRewards => _saveData!.GameRequestsRewards;

    
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

    public void AddRequest(GameRequest request) {
        GameRequests.Add(request);
        Save();
    }

    public void RemoveRequest(int index) {
        GameRequests.RemoveAt(index);
        Save();
    }

    public void ResetRequests() {
        GameRequests.Clear();
        Save();
    }
    
    public void AddReward(string id) {
        GameRequestsRewards.Add(id);
        Save();
    }

    public void ResetRewards() {
        GameRequestsRewards.Clear();
        Save();
    }
    
    public void SetFilter(GameRequestsFilter filter) {
        _saveData!.GameRequestsFilter = filter;
        Save();
    }

    public void SetFilterPlatform(string platform) {
        GameRequestsFilter.Platforms = platform;
        Save();
    }
    
    public void SetFilterMinYear(string year) {
        GameRequestsFilter.RangeYear.Min = year;
        Save();
    }
    
    public void SetFilterMaxYear(string year) {
        GameRequestsFilter.RangeYear.Max = year;
        Save();
    }
    
    public void SetFilterMinTime(int time) {
        GameRequestsFilter.RangeTime.Min = time;
        Save();
    }
    
    public void SetFilterMaxTime(int time) {
        GameRequestsFilter.RangeTime.Max = time;
        Save();
    }
}