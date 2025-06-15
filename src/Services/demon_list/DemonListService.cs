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
    
    public async Task<RecordInfo?> GetEasiest(UserProfile? profile) {
        try {
            var completed = await AredlUtils.ListUserRecords(profile?.user?.id!, _logger);
            RecordInfo? easiest = null;
            if (completed?.records.Count > 0) {
                easiest = completed.records[^1];
                var i = 2;
                while (easiest!.level.legacy && completed?.records.Count > i) {
                    easiest = completed?.records[^i++];
                }
                if (easiest.level.legacy) {
                    easiest = null;
                }
            }
            if (completed?.verified.Count > 0 && (easiest?.level.position < completed.verified[^1].level.position && !completed.verified[^1].level.legacy)) {
                easiest = completed.verified[^1];
                var i = 2;
                while (easiest!.level.legacy && completed.verified.Count > i) {
                    easiest = completed.verified[^i++];
                }
                
                if (easiest.level.legacy) {
                    easiest = null;
                }
            }
            return easiest;
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
    
    public async Task<LevelInfo?> GetRandomLevel(int from = -1, int to = -1) {
        try {
            if (from < 0) {
                from = 0;
            }
            
            var levelList = await AredlUtils.ListLevels(_logger);
            if (to < from || to > levelList!.data!.Count) {
                to = levelList!.data!.Count;
            }
            if (from > to) {
                from = 0;
            }
            var randomIndex = Random.Shared.Next(from, to);
            return levelList.data?[randomIndex];
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetRandomPlatformerLevel(int from = -1, int to = -1) {
        try {
            if (from < 0) {
                from = 0;
            }
            
            var levelList = await AredlUtils.ListPlatformerLevels(_logger);
            if (to < from || to > levelList!.data!.Count) {
                to = levelList!.data!.Count;
            }
            if (from > to) {
                from = 0;
            }
            var randomIndex = Random.Shared.Next(from, to);
            return levelList?.data?[randomIndex];
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