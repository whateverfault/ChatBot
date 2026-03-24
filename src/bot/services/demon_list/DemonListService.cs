using ChatBot.api.aredl;
using ChatBot.api.aredl.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using Range = ChatBot.api.basic.Range;

namespace ChatBot.bot.services.demon_list;

public class DemonListService : Service {
    private readonly AredlClient _aredlClient = new AredlClient(true, Network.HttpClient);
    
    public override DemonListOptions Options { get; } = new DemonListOptions();
    
    
    public void ResetCache() {
        _aredlClient.ResetCache();
    }

    public async Task<List<LevelInfo>> ListLevels(EventHandler<string>? errorCallback = null) {
        return await _aredlClient.ListLevels(errorCallback) ?? [];
    }
    
    public async Task<List<LevelInfo>> ListPlatformerLevels(EventHandler<string>? errorCallback = null) {
        return await _aredlClient.ListPlatformerLevels(errorCallback) ?? [];
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

    public List<T> GetHardests<T>(List<T> records, Range top) {
        var exceed = top.End - records.Count;
        if (exceed < 0) exceed = 0;
        
        var end = Math.Max(top.End - exceed, 0);
        end = Math.Min(end, records.Count);
        top.SetEnd(end);
        
        return records[top.Start..top.End];
    }
    
    public List<RecordInfo> GetHardests(List<RecordInfo> records, Range top) {
        return GetHardests<RecordInfo>(records.OrderBy(x => x.Level.Position).ToList(), top);
    }
    
    public List<LevelInfo> GetHardests(List<LevelInfo> records, Range top) {
        return GetHardests<LevelInfo>(records.OrderBy(x => x.Position).ToList(), top);
    }
    
    public RecordInfo GetHardest(List<RecordInfo> records) {
        return GetHardests(records, new Range(0, 1))[0];
    }
    
    public List<T> GetEasiests<T>(List<T> records, Range top) {
        var exceed = top.End - records.Count;
        if (exceed < 0) exceed = 0;
        
        var end = Math.Max(top.End - exceed, 0);
        
        end = Math.Min(end, records.Count - 1);
        
        top.SetEnd(end);
        
        return records[top.Start..top.End];
    }
    
    public List<RecordInfo> GetEasiests(List<RecordInfo> records, Range top) {
        return GetEasiests<RecordInfo>(records.OrderByDescending(x => x.Level.Position).ToList(), top);
    }
    
    public List<LevelInfo> GetEasiests(List<LevelInfo> records, Range top) {
        return GetEasiests<LevelInfo>(records.OrderByDescending(x => x.Position).ToList(), top);
    }
    
    public RecordInfo GetEasiest(List<RecordInfo> records) {
        return GetEasiests(records, new Range(0, 1))[0];
    }
    
    public async Task<List<RecordInfo>?> GetHardests(UserProfile profile, Range top) {
        try {
            var response = await _aredlClient.ListUserRecords(profile.User.Id, (_, message) => {
                                                                                   ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                               });

            return response == null 
                       ? null
                       : GetHardests(response.Records, top);
        }
        catch (Exception) {
            return null;
        }
    }
    
    public async Task<List<RecordInfo>?> GetEasiests(UserProfile profile, Range top) {
        try {
            var response =
                await _aredlClient.ListUserRecords(profile.User.Id,
                                                   (_, message) => {
                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
                                                   });

            if (response == null) {
                return null;
            }
            
            var records = response.Records.OrderBy(x => x.Level.Position).ToList();

            return records.Count <= 0 ?
                       null :
                       GetEasiests(records, top);
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
            
            var levelList = await ListLevels((_, message) => {
                                                 ErrorHandler.LogMessage(LogLevel.Error, message);
                                             });
            
            if (to < from || to > levelList.Count)
                to = levelList.Count;
            if (from > to) from = 1;
            
            var randomIndex = Random.Shared.Next(from-1, to);
            return levelList[randomIndex];
        } catch (Exception) {
            return null;
        }
    }

    public async Task<LevelInfo?> GetRandomPlatformerLevel(int from = -1, int to = -1) {
        try {
            if (from < 1) from = 1;

            var levelList = await ListPlatformerLevels((_, message) => {
                                                           ErrorHandler.LogMessage(LogLevel.Error, message);
                                                       });
            
            if (to < from || to > levelList.Count) {
                to = levelList.Count;
            }
            if (from > to) from = 1;
            
            var randomIndex = Random.Shared.Next(from-1, to);
            return levelList[randomIndex];
        } catch (Exception) {
            return null;
        }
    }
    
    public async Task<ClanDetails?> GetClan(string tag) {
        try {
            var clan = await _aredlClient.GetClan(tag, (_, message) => {
                                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                   });
            return clan;
        } catch (Exception) {
            return null;
        }
    }

    public async Task<RecordInfo?> GetClanRandomSubmission(string id) {
        try {
            var clan = await _aredlClient.GetClan(id, (_, message) => {
                                                                       ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                   });

            if (clan == null)
                return null;
            
            var randomIndex = Random.Shared.Next(0, clan.Records.Count);
            return clan.Records[randomIndex];
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
    
    public async Task<List<RecordInfo>?> GetPlatformerHardests(UserProfile profile, Range top) {
        try {
            var response = await _aredlClient.ListUserPlatformerRecords(profile.User.Id, (_, message) => {
                                                                                             ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                                         });

            return response == null 
                       ? null 
                       : GetHardests(response.Records, top);
        }
        catch (Exception) {
            return null;
        }
    }
    
    public async Task<List<RecordInfo>?> GetPlatformerEasiests(UserProfile profile, Range top) {
        try {
            var response =
                await _aredlClient.ListUserPlatformerRecords(profile.User.Id, (_, message) => {
                                                                                  ErrorHandler.LogMessage(LogLevel.Error, message);
                                                                              });

            return response == null 
                       ? null 
                       : GetEasiests(response.Records, top);
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

    public bool GetDefaultUserName(out string userName) {
        userName = Options.GetDefaultUserName();
        return Options.GetUseDefaultUserName();
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
    
    public string FormatClanDetails(ClanDetails clan) {
        var formated = $"#{clan.Rank.Rank} [{clan.Clan.Tag}] {clan.Clan.GlobalName} | aredl.net/profile/clan/{clan.Clan.Id}";
        return formated;
    }

    public string FormatUserProfileInfo(UserProfile profile) {
        var formated = $"#{profile.Rank} {profile.User.GlobalName} | aredl.net/profile/user/{profile.User.UserName}";
        return formated;
    }
    
    private string FormatAdditionalLevelInfo(LevelInfo levelInfo) {
        var tier = 
            levelInfo.NlwTier == null? 
                string.Empty:
                $"({levelInfo.NlwTier} tier";
        
        var enjoyment = (levelInfo.EdelEnjoyment == null) switch {
                            true  => string.IsNullOrEmpty(tier) ? string.Empty : ")",
                            false => string.IsNullOrEmpty(tier) ? $"(EDEL: {(int)levelInfo.EdelEnjoyment})" : $"; EDEL: {(int)levelInfo.EdelEnjoyment})",
                        };

        return $"{tier}{enjoyment}";
    }
}