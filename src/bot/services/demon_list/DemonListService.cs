using ChatBot.api.aredl;
using ChatBot.api.aredl.data;
using ChatBot.api.twitch.client;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.demon_list;

public class DemonListService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);

    private AredlClient? _aredlClient;

    public override string Name => ServiceName.DemonList;
    public override DemonListOptions Options { get; } = new DemonListOptions();


    public void ResetCache() {
        _aredlClient?.ResetCache();
    }
    
    public async Task<LevelInfo?> GetLevelByPlacement(int placement) {
        try {
            if (_aredlClient == null) return null;

            var level =
                await _aredlClient.GetLevelByPlacement(placement,
                                                       (_, message) => { _logger.Log(LogLevel.Error, message); });
            return level;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<List<LevelInfo>?> GetLevelsInfoByName(string levelName) {
        try {
            if (_aredlClient == null) return null;

            var level =
                await _aredlClient.GetLevelsByName(levelName,
                                                   (_, message) => { _logger.Log(LogLevel.Error, message); });
            return level;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<LevelInfo?> GetLevelInfoByName(string levelName, string? creator = null) {
        try {
            if (_aredlClient == null) return null;

            var level = await _aredlClient.GetLevelByName(levelName, creator,
                                                          (_, message) => { _logger.Log(LogLevel.Error, message); });
            return level;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<UserProfile?> GetProfile(string username) {
        try {
            if (_aredlClient == null) return null;

            var profile =
                await _aredlClient.FindProfile(username, (_, message) => { _logger.Log(LogLevel.Error, message); });
            return profile;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<RecordInfo?> GetHardest(UserProfile profile) {
        try {
            if (_aredlClient == null) return null;
            if (profile.Hardest == null
             || profile.User == null) return null;

            var hardest = await _aredlClient.GetRecord(profile.Hardest.id, profile.User.id,
                                                       (_, message) => {
                                                           _logger.Log(LogLevel.Error, message);
                                                       });
            return hardest;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<LevelInfo?> GetHardest() {
        try {
            if (_aredlClient == null) return null;

            var levels = await _aredlClient.ListLevels();
            if (levels?.Data == null || levels.Data.Count < 1) {
                return null;
            }

            return levels.Data[0];
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<RecordInfo?> GetEasiest(UserProfile? profile) {
        try {
            if (_aredlClient == null) return null;
            
            var completed =
                await _aredlClient.ListUserRecords(profile?.User?.id!,
                                                   (_, message) => { _logger.Log(LogLevel.Error, message); });
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
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<LevelInfo?> GetEasiest() {
        try {
            if (_aredlClient == null) return null;
            
            var levels = await _aredlClient.ListLevels();
            if (levels?.Data == null || levels.Data.Count < 1) {
                return null;
            }

            var level = levels.Data[^1];
            var i = 2;
            while (level is { Legacy: true, }) {
                level = levels.Data?[^i++];
            }
            
            return level;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<string?> GetLevelVerificationLink(string levelId) {
        try {
            if (_aredlClient == null) return null;
            
            var levelDetails = await _aredlClient.GetLevelDetails(levelId, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });

            return levelDetails?.verifications.Count >= 1 ? 
                       levelDetails.verifications[0].videoUrl 
                       : null;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<LevelDetails?> GetLevelDetails(string levelId) {
        try {
            if (_aredlClient == null) return null;
            
            var levelDetails = await _aredlClient.GetLevelDetails(levelId, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return levelDetails;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetRandomLevel(int from = -1, int to = -1) {
        try {
            if (from < 1) from = 1;
            
            if (_aredlClient == null) return null;
            
            var levelList = await _aredlClient.ListLevels((_, message) => { 
                                                       _logger.Log(LogLevel.Error, message);
                                                   });
            if (levelList == null) return null;
            
            if (to < from || to > levelList.Data.Count) {
                to = levelList.Data.Count;
            }
            if (from > to) from = 1;
            var randomIndex = Random.Shared.Next(from-1, to);
            return levelList.Data[randomIndex];
        } catch (Exception) {
            return null;
        }
    }

    public async Task<ClanInfo?> GetClanInfo(string tag) {
        try {
            if (_aredlClient == null) return null;
            
            var clan = await _aredlClient.GetClan(tag, (_, message) => {
                                                                       _logger.Log(LogLevel.Error, message);
                                                                   });
            return clan;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<ClanSubmissionInfo?>? GetRandomClanSubmission(string id) {
        try {
            if (_aredlClient == null) return null;
            
            var clan = await _aredlClient.GetClanRecords(id, (_, message) => {
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
    
    public async Task<LevelInfo?> GetPlatformerLevelByPlacement(int placement) {
        try {
            if (_aredlClient == null) return null;

            var level =
                await _aredlClient.GetPlatformerLevelByPlacement(placement,
                                                                 (_, message) => {
                                                                     _logger.Log(LogLevel.Error, message);
                                                                 });
            return level;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<List<LevelInfo>?> GetPlatformerLevelsInfoByName(string levelName) {
        try {
            if (_aredlClient == null) return null;

            var level =
                await _aredlClient.GetPlatformerLevelsByName(levelName,
                                                             (_, message) => { _logger.Log(LogLevel.Error, message); });
            return level;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<LevelInfo?> GetPlatformerLevelInfoByName(string levelName, string? creator = null) {
        try {
            if (_aredlClient == null) return null;

            var level = await _aredlClient.GetPlatformerLevelByName(levelName, creator,
                                                                    (_, message) => {
                                                                        _logger.Log(LogLevel.Error, message);
                                                                    });
            return level;
        }
        catch (Exception) {
            return null;
        }
    }
    
    public async Task<string?> GetPlatformerLevelVerificationLink(string levelId) {
        try {
            if (_aredlClient == null) return null;
            
            var levelDetails = await _aredlClient.GetPlatformerLevelDetails(levelId, (_, message) => {
                                                                                _logger.Log(LogLevel.Error, message);
                                                                            });
            return levelDetails?.verifications.Count >= 1 ? 
                       levelDetails.verifications[0].videoUrl:
                       null;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelDetails?> GetPlatformerLevelDetails(string levelId) {
        try {
            if (_aredlClient == null) return null;
            
            var levelDetails = await _aredlClient.GetPlatformerLevelDetails(levelId, (_, message) => {
                                                                               _logger.Log(LogLevel.Error, message);
                                                                           });
            return levelDetails;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<RecordInfo?> GetPlatformerHardest(UserProfile profile) {
        try {
            if (_aredlClient == null) return null;
            if (profile.Hardest == null
             || profile.User == null) return null;
            
            var levelRecords = await _aredlClient.ListUserPlatformerRecords(profile.User.id, (_, message) => {
                                                                                _logger.Log(LogLevel.Error, message);
                                                                            });
            if (levelRecords == null) return null;
            
            if (levelRecords.records.Count + levelRecords.verified.Count < 1) {
                _logger.Log(LogLevel.Error, "Error while fetching user record data.");
                return null;
            }

            var max = int.MinValue;
            RecordInfo? recordInfo = null;

            foreach (var level in levelRecords.verified) {
                if (level.level.position <= max) continue;

                recordInfo = level;
                max = level.level.position;
            }
            
            foreach (var level in levelRecords.records) {
                if (level.level.position <= max) continue;

                recordInfo = level;
                max = level.level.position;
            }
            
            return recordInfo;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<LevelInfo?> GetPlatformerHardest() {
        try {
            if (_aredlClient == null) return null;

            var levels = await _aredlClient.ListPlatformerLevels();
            if (levels?.Data == null || levels.Data.Count < 1) {
                return null;
            }

            return levels.Data[0];
        }
        catch (Exception) {
            return null;
        }
    }
    
        public async Task<RecordInfo?> GetPlatformerEasiest(UserProfile? profile) {
        try {
            if (_aredlClient == null) return null;
            if (profile?.User == null) return null;
            
            var completed =
                await _aredlClient.ListUserPlatformerRecords(profile.User.id, 
                                                             (_, message) => { _logger.Log(LogLevel.Error, message); });
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
                if (!(easiestVerification.level.position < verification.level.position)) continue;
                if (verification.level.legacy) continue;
                easiestVerification = verification;
            }
            
            if (easiest == null || easiest.level.position < easiestVerification.level.position) {
                easiest = easiestVerification;
            }
            
            return easiest;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<LevelInfo?> GetPlatformerEasiest() {
        try {
            if (_aredlClient == null) return null;
            
            var levels = await _aredlClient.ListPlatformerLevels();
            if (levels?.Data == null || levels.Data.Count < 1) {
                return null;
            }

            var level = levels.Data[^1];
            var i = 2;
            while (level is { Legacy: true, }) {
                level = levels.Data?[^i++];
            }
            
            return level;
        }
        catch (Exception) {
            return null;
        }
    }
    
    public override void Init() {
        base.Init();
        
        _aredlClient = new AredlClient(true);
    }
}