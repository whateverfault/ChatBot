using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.Static;
using ChatBot.utils.GD.AREDL;
using ChatBot.utils.GD.AREDL.Data;

namespace ChatBot.Services.demon_list;

public class DemonListService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    public override string Name => ServiceName.DemonList;
    public override DemonListOptions Options { get; } = new();


    public async Task<string?> GetLevelNameByPlacement(int placement) {
        try {
            var level = await AredlUtils.GetLevelByPlacement(placement, _logger);
            return level?.name;
        } catch (Exception) {
            _logger.Log(LogLevel.Error, "Could not Get Level Name By Placement");
            return null;
        }
    }

    public async Task<int?> GetLevelPlacementByName(string levelName, string? creator = null) {
        try {
            var level = await AredlUtils.GetLevelByName(levelName, creator, _logger);
            return level?.position;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetLevelInfoByName(string levelName, string? creator = null) {
        try {
            var level = await AredlUtils.GetLevelByName(levelName, creator, _logger);
            return level;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<string?> GetPlatformerLevelNameByPlacement(int placement) {
        try {
            var level = await AredlUtils.GetPlatformerLevelByPlacement(placement, _logger);
            return level?.name;
        } catch (Exception) {
            _logger.Log(LogLevel.Error, "Could not Get Level Name By Placement");
            return null;
        }
    }

    public async Task<int?> GetPlatformerLevelPlacementByName(string levelName, string? creator = null) {
        try {
            var level = await AredlUtils.GetPlatformerLevelByName(levelName, creator, _logger);
            return level?.position;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetPlatformerLevelInfoByName(string levelName, string? creator = null) {
        try {
            var level = await AredlUtils.GetPlatformerLevelByName(levelName, creator, _logger);
            return level;
        } catch (Exception) {
            return null;
        }
    }    
    
    public async Task<UserProfile?> GetUserProfile(string username) {
        try {
            var profile = await AredlUtils.FindUserProfile(username, _logger);
            return profile;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<SubmissionInfo?> GetUserHardest(UserProfile? profile) {
        try {
            var hardest = await AredlUtils.GetUserRecord(profile?.hardest?.id!, profile?.user?.id!, _logger);
            return hardest;
        } catch (Exception) {
            return null;
        }
    }
}