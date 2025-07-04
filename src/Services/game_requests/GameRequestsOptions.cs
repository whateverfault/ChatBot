using ChatBot.Services.game_requests.Data;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.game_requests;

public class GameRequestsOptions : Options {
    private SaveData? _saveData;
    
    private string FilterPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_filters.json");

    private List<GameRequest>? _gameRequests = [];
    private string GameRequestsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}.json");

    protected override string Name => "game_requests";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<GameRequest>? GameRequests => _gameRequests;
    public string RawgApiKey => _saveData!.RawgApiKey;
    public List<string> GameRequestsRewards => _saveData!.GameRequestsRewards;

    
    public override bool TryLoad() {
        return JsonUtils.TryRead(OptionsPath, out _saveData) &&
               JsonUtils.TryRead(GameRequestsPath, out _gameRequests);
    }

    public override void Load() {
        var anythingWentWrong = false;
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            anythingWentWrong = true;
        } if (!JsonUtils.TryRead(GameRequestsPath, out _gameRequests!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            anythingWentWrong = true;
        }

        if (anythingWentWrong) {
            SetDefaults();
        }
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
        JsonUtils.WriteSafe(GameRequestsPath, Path.Combine(Directories.ServiceDirectory, Name), _gameRequests);
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        _gameRequests = [];
        Save();
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }

    public void AddRequest(GameRequest request, int position) {
        GameRequests?.Insert(position, request);
        Save();
    }

    public void RemoveRequest(int index) {
        GameRequests?.RemoveAt(index);
        Save();
    }

    public void ResetRequests() {
        GameRequests?.Clear();
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
}