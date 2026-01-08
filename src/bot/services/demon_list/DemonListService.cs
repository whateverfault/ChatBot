using ChatBot.api.aredl;
using ChatBot.api.aredl.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;

namespace ChatBot.bot.services.demon_list;

public class DemonListService : Service {
    private readonly AredlClient _aredlClient = new AredlClient(true, Network.HttpClient);
    
    public override DemonListOptions Options { get; } = new DemonListOptions();

    public int LevelsCount => _aredlClient.LevelsCount;
    

    public void ResetCache() {
        _aredlClient.ResetCache();
    }
    
    public async Task<LevelInfo?> GetLevelByPlacement(int placement) {
        try {
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
            var profile =
                await _aredlClient.ListProfiles(username, (_, message) => {
                                                              ErrorHandler.LogMessage(LogLevel.Error, message);
                                                          });

            if (profile == null
             || profile.Data.Count <= 0) {
                return null;
            }
            
            return profile.Data[0];
        }
        catch (Exception) {
            return null;
        }
    }
    
    public async Task<List<RecordInfo>?> GetHardests(UserProfile profile, int top) {
        if (top <= 0) return null;
        
        try {
            if (profile.Hardest == null
             || profile.User == null) return null;
            
            var response = await _aredlClient.ListUserRecords(profile.User.Id, (_, message) => {
                                                                                  ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                              });

            if (response == null) {
                return null;
            }
            
            var records = response.Records.Concat(response.Verified).OrderBy(x => x.Level.Position).ToList();

            top = Math.Min(top, records.Count);
            
            return records[0..top];
        }
        catch (Exception) {
            return null;
        }
    }
    
    public async Task<RecordInfo?> GetHardest(UserProfile profile) {
        try {
            if (profile.Hardest == null
             || profile.User == null) return null;

            var hardest = await _aredlClient.GetRecord(profile.Hardest.Id, profile.User.Id,
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

    public async Task<List<RecordInfo>?> GetEasiests(UserProfile profile, int top) {
        if (top <= 0 || profile.User == null) {
            return null;
        }
        
        try {
            var response =
                await _aredlClient.ListUserRecords(profile.User.Id,
                                                   (_, message) => {
                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
                                                   });

            if (response == null) {
                return null;
            }
            
            var records = response.Records.Concat(response.Verified).OrderBy(x => x.Level.Position).ToList();

            if (records.Count <= 0) {
                return null;
            }

            top = Math.Min(top, records.Count);
            var easiests = records[^top..records.Count];
            easiests.Reverse();
            
            return easiests;
        }
        catch (Exception) {
            return null;
        }
    }

    public async Task<string?> GetLevelVerificationLink(string levelId) {
        try {
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
            var clan = await _aredlClient.GetClanRecords(id, (_, message) => {
                                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                   });
            var randomIndex = Random.Shared.Next(0, clan!.Records.Count+clan.Verified.Count);
            return randomIndex < clan.Records.Count?
                       clan.Records[randomIndex] :
                       clan.Verified[randomIndex];
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<UserProfile?> GetPlatformerProfile(string username) {
        try {
            var profile =
                await _aredlClient.ListPlatformerProfiles(username, (_, message) => { ErrorHandler.LogMessage(LogLevel.Error, message); });

            if (profile == null
             || profile.Data.Count <= 0) {
                return null;
            }
            
            return profile.Data[0];
        }
        catch (Exception) {
            return null;
        }
    }
    
    public async Task<LevelInfo?> GetPlatformerLevelByPlacement(int placement) {
        try {
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
            var level = await _aredlClient.GetPlatformerLevelDetails(levelId, (_, message) => {
                                                                               ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                           });
            
            return level;
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<List<RecordInfo>?> GetPlatformerHardests(UserProfile profile, int top) {
        if (top <= 0) return null;
        
        try {
            if (profile.Hardest == null
             || profile.User == null) return null;
            
            var response = await _aredlClient.ListUserPlatformerRecords(profile.User.Id, (_, message) => {
                                                                                   ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                               });

            if (response == null) {
                return null;
            }
            
            var records = response.Records.Concat(response.Verified).OrderBy(x => x.Level.Position).ToList();

            top = Math.Min(top, records.Count);
            
            return records[0..top];
        }
        catch (Exception) {
            return null;
        }
    }
    
    public async Task<List<RecordInfo>?> GetPlatformerEasiests(UserProfile profile, int top) {
        if (top <= 0 || profile.User == null) {
            return null;
        }
        
        try {
            var response =
                await _aredlClient.ListUserPlatformerRecords(profile.User.Id, (_, message) => {
                                                                                  ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                              });

            if (response == null) {
                return null;
            }
            
            var records = response.Records.Concat(response.Verified).OrderBy(x => x.Level.Position).ToList();

            if (records.Count <= 0) {
                return null;
            }

            top = Math.Min(top, records.Count);
            var easiests = records[^top..records.Count];
            easiests.Reverse();
            
            return easiests;
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
    
    public async Task<string> FormatLevelInfo(LevelInfo levelInfo, bool withLink = true) {
        var verificationLink = string.Empty;

        if (withLink) {
            verificationLink = levelInfo.Platformer switch {
                true  => await GetPlatformerLevelVerificationLink(levelInfo.Id),
                false => await GetLevelVerificationLink(levelInfo.Id),
            };
        }

        verificationLink = string.IsNullOrEmpty(verificationLink)? 
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
    
    public string FormatRecordInfo(RecordInfo recordInfo, bool withLink = true) {
        var formated = $"#{recordInfo.Level.Position} {recordInfo.Level.Name} {(withLink ? $"| {recordInfo.VideoUrl}" : string.Empty)}";
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

    public string FormatUserProfileInfo(UserProfile profile) {
        if (profile.User == null) {
            return string.Empty;
        }
        
        var formated = $"#{profile.Rank} {profile.User.GlobalName} | https://aredl.net/profile/user/{profile.User.Username}";
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
}