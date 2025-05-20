using ChatBot.Shared;
using ChatBot.Shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.game_requests;

public class GameRequestsOptions : Options{
    protected override string Name => "game_requests";
    protected override string OptionsPath => Path.Combine(Directories.serviceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => saveData!.state;
    public SaveData? saveData = new();

    
    public override bool Load() {
        if (!Path.Exists(OptionsPath)) return false;
        JsonUtils.TryRead(OptionsPath, out saveData);
        return true;
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.serviceDirectory, Name), saveData);
    }
    
    public void SetDefaults() {
        saveData!.gameRequests = [];
        saveData.gameRequestsPoints = [];
    }

    public void SetState(State state) {
        saveData!.state = state;
    }
}