using ChatBot.api.demon_list.data;

namespace ChatBot.api.demon_list;

public interface IDemonList {
    public void ResetCache();

    public Task<UserProfile?> GetProfile(string username, EventHandler<string>? errorCallback = null);
    
    public Task<List<LevelInfo>?> ListLevels(EventHandler<string>? errorCallback = null);
    public Task<List<LevelInfo>?> GetLevelsByName(string name, EventHandler<string>? errorCallback = null);
    public Task<LevelInfo?> GetLevelByName(string name, string? creator = null, EventHandler<string>? errorCallback = null);
    public Task<LevelInfo?> GetLevelByPlacement(int placement, EventHandler<string>? errorCallback = null);
}