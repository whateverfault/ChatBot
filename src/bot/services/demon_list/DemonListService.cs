using ChatBot.api.aredl;
using ChatBot.api.aredl.data;
using ChatBot.api.twitch.client;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.demon_list;

public class DemonListService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    public override string Name => ServiceName.DemonList;
    public override DemonListOptions Options { get; } = new DemonListOptions();


    public async Task<LevelInfo?> GetLevelByPlacement(int placement) {
        try {
            var level = await Aredl.GetLevelByPlacement(placement, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return level;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<List<LevelInfo>?> GetLevelsInfoByName(string levelName) {
        try {
            var level = await Aredl.GetLevelsByName(levelName, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return level;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetLevelInfoByName(string levelName, string? creator = null) {
        try {
            var level = await Aredl.GetLevelByName(levelName, creator, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return level;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<LevelInfo?> GetPlatformerLevelByPlacement(int placement) {
        try {
            var level = await Aredl.GetPlatformerLevelByPlacement(placement, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return level;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<List<LevelInfo>?> GetPlatformerLevelsInfoByName(string levelName) {
        try {
            var level = await Aredl.GetPlatformerLevelsByName(levelName, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return level;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetPlatformerLevelInfoByName(string levelName, string? creator = null) {
        try {
            var level = await Aredl.GetPlatformerLevelByName(levelName, creator, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return level;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<UserProfile?> GetProfile(string username) {
        try {
            var profile = await Aredl.FindProfile(username, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return profile;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<RecordInfo?> GetHardest(UserProfile? profile) {
        try {
            var hardest = await Aredl.GetRecord(profile?.hardest?.id!, profile?.user?.id!, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return hardest;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<RecordInfo?> GetEasiest(UserProfile? profile) {
        try {
            var completed = await Aredl.ListUserRecords(profile?.user?.id!, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            RecordInfo? easiest = null;
            if (completed?.records.Count > 0) {
                easiest = completed.records[^1];
                var i = 2;
                while (easiest.level.legacy && completed.records.Count > i) {
                    easiest = completed.records[^i++];
                }
                if (easiest.level.legacy) {
                    easiest = null;
                }
            }
            if (!(completed?.verified.Count > 0)) return easiest;
            
            var easiestVerification = completed.verified[0];
            foreach (var verification in completed.verified) {
                if (!(easiestVerification?.level.position < verification.level.position)) continue;
                if (verification.level.legacy) continue;
                easiestVerification = verification;
            }
            
            if (easiest?.level.position < easiestVerification?.level.position || easiest == null) {
                easiest = easiestVerification;
            }
            return easiest;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<string?> GetLevelVerificationLink(string levelId) {
        try {
            var levelDetails = await Aredl.GetLevelDetails(levelId, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
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
            var levelDetails = await Aredl.GetPlatformerLevelDetails(levelId, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
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
            var levelDetails = await Aredl.GetLevelDetails(levelId, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return levelDetails;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelDetails?> GetPlatformerLevelDetails(string levelId) {
        try {
            var levelDetails = await Aredl.GetPlatformerLevelDetails(levelId, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return levelDetails;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetRandomLevel(int from = -1, int to = -1) {
        try {
            if (from < 1) {
                from = 1;
            }
            
            var levelList = await Aredl.ListLevels((_, message) => { 
                                                       _logger.Log(LogLevel.Error, message);
                                                   });
            if (to < from || to > levelList!.data!.Count) {
                to = levelList!.data!.Count;
            }
            if (from > to) {
                from = 1;
            }
            var randomIndex = Random.Shared.Next(from-1, to);
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
            
            var levelList = await Aredl.ListPlatformerLevels((_, message) => { 
                                                                 _logger.Log(LogLevel.Error, message);
                                                             });
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

    public async Task<ClanInfo?> GetClanInfo(string tag) {
        try {
            var clan = await Aredl.GetClan(tag, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return clan;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<ClanSubmissionInfo?>? GetRandomClanSubmission(string id) {
        try {
            var clan = await Aredl.GetClanRecords(id, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            var randomIndex = Random.Shared.Next(0, clan!.records.Count+clan.verified.Count);
            return randomIndex < clan.records.Count?
                       clan.records[randomIndex] :
                       clan.verified[randomIndex];
        } catch (Exception) {
            return null;
        }
    }
}