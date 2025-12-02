using ChatBot.api.aredl;
using ChatBot.api.aredl.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;

namespace ChatBot.bot.services.demon_list;

public class DemonListService : Service {
    private AredlClient? _aredlClient;
    
    public override DemonListOptions Options { get; } = new DemonListOptions();


    public void ResetCache() {
        _aredlClient?.ResetCache();
    }
    
    public async Task<LevelInfo?> GetLevelByPlacement(int placement) {
        try {
            if (_aredlClient == null) return null;

            var level =
                await _aredlClient.GetLevelByPlacement(placement,
                                                       (_, message) => {
                                                           ErrorHandler.LogMessage(LogLevel.Error, message);
                                                       });
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
                                                   (_, message) => {
                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
                                                   });
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
                                                          (_, message) => {
                                                              ErrorHandler.LogMessage(LogLevel.Error, message);
                                                          });
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
                await _aredlClient.FindProfile(username, (_, message) => { ErrorHandler.LogMessage(LogLevel.Error, message); });
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

            var hardest = await _aredlClient.GetRecord(profile.Hardest.Id, profile.User.id,
                                                       (_, message) => {
                                                           ErrorHandler.LogMessage(LogLevel.Error, message);
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
                                                   (_, message) => { ErrorHandler.LogMessage(LogLevel.Error, message); });
            RecordInfo? easiest = null;
            if (completed?.records.Count > 0) {
                easiest = completed.records[^1];
                var i = 2;
                while (easiest.Level.Legacy && completed.records.Count > i) {
                    easiest = completed.records[^i++];
                }

                if (easiest.Level.Legacy) {
                    easiest = null;
                }
            }

            if (!(completed?.verified.Count > 0)) return easiest;

            var easiestVerification = completed.verified[0];
            foreach (var verification in completed.verified) {
                if (!(easiestVerification?.Level.Position < verification.Level.Position)) continue;
                if (verification.Level.Legacy) continue;
                easiestVerification = verification;
            }

            if (easiest?.Level.Position < easiestVerification?.Level.Position || easiest == null) {
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
                                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                   });

            return levelDetails?.Verifications.Count >= 1 ? 
                       levelDetails.Verifications[0].VideoUrl 
                       : null;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<LevelDetails?> GetLevelDetails(string levelId) {
        try {
            if (_aredlClient == null) return null;
            
            var levelDetails = await _aredlClient.GetLevelDetails(levelId, (_, message) => {
                                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
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
                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
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
                                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
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
                                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
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
                                                                     ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                 });
            if (level != null) level.Platformer = true;
            
            return level;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<List<LevelInfo>?> GetPlatformerLevelsInfoByName(string levelName) {
        try {
            if (_aredlClient == null) return null;

            var levels =
                await _aredlClient.GetPlatformerLevelsByName(levelName,
                                                             (_, message) => { ErrorHandler.LogMessage(LogLevel.Error, message); });
            if (levels == null) return levels;

            foreach (var level in levels) level.Platformer = true;
            return levels;
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
                                                                        ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                    });
            if (level != null) level.Platformer = true;
            
            return level;
        }
        catch (Exception) {
            return null;
        }
    }
    
    public async Task<string?> GetPlatformerLevelVerificationLink(string levelId) {
        try {
            if (_aredlClient == null) return null;
            
            var level = await _aredlClient.GetPlatformerLevelDetails(levelId, (_, message) => {
                                                                                ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                            });
            if (level == null 
             || level.Verifications.Count <= 0) {
                return null;
            }

            return level.Verifications[0].VideoUrl;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelDetails?> GetPlatformerLevelDetails(string levelId) {
        try {
            if (_aredlClient == null) return null;
            
            var level = await _aredlClient.GetPlatformerLevelDetails(levelId, (_, message) => {
                                                                               ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                           });
            
            return level;
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
                                                                                ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                            });
            if (levelRecords == null) return null;
            
            if (levelRecords.records.Count + levelRecords.verified.Count < 1) {
                ErrorHandler.LogMessage(LogLevel.Error, "Error while fetching user record data.");
                return null;
            }

            var max = int.MinValue;
            RecordInfo? recordInfo = null;

            foreach (var level in levelRecords.verified) {
                if (level.Level.Position <= max) continue;

                recordInfo = level;
                max = level.Level.Position;
            }
            
            foreach (var level in levelRecords.records) {
                if (level.Level.Position <= max) continue;

                recordInfo = level;
                max = level.Level.Position;
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
            if (levels?.Data == null
             || levels.Data.Count < 1) {
                return null;
            }

            var level = levels.Data[0];
            level.Platformer = true;
            
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
                                                             (_, message) => { ErrorHandler.LogMessage(LogLevel.Error, message); });
            RecordInfo? easiest = null;
            if (completed?.records.Count > 0) {
                easiest = completed.records[^1];
                var i = 2;
                while (easiest.Level.Legacy && completed.records.Count > i) {
                    easiest = completed.records[^i++];
                }

                if (easiest.Level.Legacy) {
                    easiest = null;
                }
            }

            if (!(completed?.verified.Count > 0)) return easiest;

            var easiestVerification = completed.verified[0];
            foreach (var verification in completed.verified) {
                if (!(easiestVerification.Level.Position < verification.Level.Position)) continue;
                if (verification.Level.Legacy) continue;
                easiestVerification = verification;
            }
            
            if (easiest == null || easiest.Level.Position < easiestVerification.Level.Position) {
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

            if (level != null) level.Platformer = true;
            if (level != null) level.Platformer = true;
            return level;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<LevelInfo?> ClanSubmissionInfoToLevelInfo(ClanSubmissionInfo clanSubmissionInfo) {
        if (clanSubmissionInfo.Level == null) {
            return null;
        }
        
        var levelsInfo = await GetLevelsInfoByName(clanSubmissionInfo.Level.Name);
        if (levelsInfo == null) {
            return null;
        }

        levelsInfo = levelsInfo
                    .Where(x => x.Id
                                 .Equals(clanSubmissionInfo.Level.Id))
                    .ToList();

        return levelsInfo.FirstOrDefault();
    }
    
    public async Task<string> FormatLevelInfo(LevelInfo levelInfo) {
        var verificationLink = levelInfo.Platformer switch {
                                   true  => await GetPlatformerLevelVerificationLink(levelInfo.Id),
                                   false => await GetLevelVerificationLink(levelInfo.Id),
                               };

        verificationLink = string.IsNullOrEmpty(verificationLink) ? 
                               string.Empty :
                               $"| {verificationLink}";
        
        var formatedAdditionalLevelInfo = FormatAdditionalLevelInfo(levelInfo);
        var formated = $"#{levelInfo.Position} {levelInfo.Name} {formatedAdditionalLevelInfo} {verificationLink}";
        
        return formated;
    }
    
    public string FormatLevelDetails(LevelDetails levelDetails) {
        var formated = $"#{levelDetails.Position} {levelDetails.Name} | {levelDetails.Verifications[0].VideoUrl}";
        return formated;
    }
    
    public string FormatRecordInfo(RecordInfo recordInfo) {
        var formated = $"#{recordInfo.Level.Position} {recordInfo.Level.Name} | {recordInfo.VideoUrl}";
        return formated;
    }
    
    public string FormatClanInfo(ClanInfo clanInfo) {
        var formated = $"#{clanInfo.Rank} [{clanInfo.Clan.Tag}] {clanInfo.Clan.GlobalName} | https://aredl.net/clans/{clanInfo.Clan.Id}";
        return formated;
    }
    
    public string FormatClanSubmissionInfo(ClanSubmissionInfo clanSubmissionInfo) {
        if (clanSubmissionInfo.Level == null) {
            return string.Empty;
        }
        
        var formated = $"#{clanSubmissionInfo.Level.Position} {clanSubmissionInfo.Level.Name} | {clanSubmissionInfo.VideoUrl}";
        return formated;
    }
    
    private string FormatAdditionalLevelInfo(LevelInfo levelInfo) {
        var tier = 
            levelInfo.NlwTier == null? 
                string.Empty:
                $"({levelInfo.NlwTier} tier";
        
        var enjoyment = (levelInfo.EdelEnjoyment == null) switch {
                            true  => string.IsNullOrEmpty(tier) ? string.Empty : ")",
                            false => string.IsNullOrEmpty(tier) ? $"(EDL: {(int)levelInfo.EdelEnjoyment})" : $"; EDL: {(int)levelInfo.EdelEnjoyment})",
                        };

        return $"{tier}{enjoyment}";
    }
    
    public override void Init() {
        base.Init();
        
        _aredlClient = new AredlClient(true);
    }
}