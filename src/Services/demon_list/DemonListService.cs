using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.Static;
using ChatBot.utils.GD.AREDL;
using ChatBot.utils.GD.AREDL.Data;
using ChatBot.utils.GD.AREDL.Responses;

namespace ChatBot.Services.demon_list;

public class DemonListService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    public override string Name => ServiceName.DemonList;
    public override DemonListOptions Options { get; } = new();


    public async Task<LevelInfo?> GetLevelByPlacement(int placement) {
        try {
            var level = await AredlUtils.GetLevelByPlacement(placement, _logger);
            return level;
        } catch (Exception) {
            _logger.Log(LogLevel.Error, "Could not Get Level Name By Placement");
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

    public async Task<LevelInfo?> GetPlatformerLevelByPlacement(int placement) {
        try {
            var level = await AredlUtils.GetPlatformerLevelByPlacement(placement, _logger);
            return level;
        } catch (Exception) {
            _logger.Log(LogLevel.Error, "Could not Get Level Name By Placement");
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
    
    public async Task<UserProfile?> GetProfile(string username) {
        try {
            var profile = await AredlUtils.FindProfile(username, _logger);
            return profile;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<RecordInfo?> GetHardest(UserProfile? profile) {
        try {
            var hardest = await AredlUtils.GetRecord(profile?.hardest?.id!, profile?.user?.id!, _logger);
            return hardest;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<UserProfile?> GetPlatformerProfile(string username) {
        try {
            var profile = await AredlUtils.FindPlatformerProfile(username, _logger);
            return profile;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<RecordInfo?> GetPlatformerHardest(UserProfile? profile) {
        try {
            var hardest = await AredlUtils.GetPlatformerRecord(profile?.hardest?.id!, profile?.user?.id!, _logger);
            return hardest;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<string?> GetLevelVerificationLink(string levelId) {
        try {
            var levelDetails = await AredlUtils.GetLevelDetails(levelId, _logger);
            if (levelDetails?.verifications.Count < 1) {
                return null;
            }
            return levelDetails?.verifications[0].videoUrl;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<string?> GetPlatformerLevelVerificationLink(string levelId) {
        try {
            var levelDetails = await AredlUtils.GetPlatformerLevelDetails(levelId, _logger);
            if (levelDetails?.verifications.Count < 1) {
                return null;
            }
            return levelDetails?.verifications[0].videoUrl;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<LevelDetails?> GetLevelDetails(string levelId) {
        try {
            var levelDetails = await AredlUtils.GetLevelDetails(levelId, _logger);
            return levelDetails;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelDetails?> GetPlatformerLevelDetails(string levelId) {
        try {
            var levelDetails = await AredlUtils.GetPlatformerLevelDetails(levelId, _logger);
            return levelDetails;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetRandomLevel() {
        try {
            var levelList = await AredlUtils.ListLevels(_logger);
            var randomIndex = Random.Shared.Next(0, levelList!.data!.Count);
            return levelList.data[randomIndex];
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetRandomPlatformerLevel() {
        try {
            var levelList = await AredlUtils.ListPlatformerLevels(_logger);
            var randomIndex = Random.Shared.Next(0, levelList!.data!.Count);
            return levelList.data[randomIndex];
        } catch (Exception) {
            return null;
        }
    }

    public async Task<ClanInfo?> GetClanInfo(string tag) {
        try {
            var clan = await AredlUtils.GetClan(tag, _logger);
            return clan;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<ClanSubmissionInfo?>? GetRandomClanSubmission(string id) {
        try {
            var clan = await AredlUtils.GetClanRecords(id, _logger);
            var randomIndex = Random.Shared.Next(0, clan!.records.Count+clan.verified.Count);
            return randomIndex < clan.records.Count?
                       clan.records[randomIndex] :
                       clan.verified[randomIndex];
        } catch (Exception) {
            return null;
        }
    }
}