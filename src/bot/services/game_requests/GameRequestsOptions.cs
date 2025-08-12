using ChatBot.bot.services.game_requests.Data;
using ChatBot.shared;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.bot.services.game_requests;

public class GameRequestsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    private List<GameRequest>? _gameRequests = [];
    private string GameRequestsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}.json");

    protected override string Name => "game_requests";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<GameRequest>? GameRequests => _gameRequests;
    public List<string> GameRequestsRewards => _saveData!.GameRequestsRewards;
    

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        } if (!JsonUtils.TryRead(GameRequestsPath, out _gameRequests!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
            JsonUtils.WriteSafe(GameRequestsPath, Path.Combine(Directories.ServiceDirectory, Name), _gameRequests);
        }
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