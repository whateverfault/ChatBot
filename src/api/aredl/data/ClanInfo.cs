using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class ClanInfo {
    public int Rank;
    public int ExtremesRank;
    public readonly ClanData Clan;
    public int LevelPoints;
    public int MemberCount;
    public readonly HardestInfo Hardest;
    public int Extremes;

    
    public ClanInfo(
        [JsonProperty(PropertyName = "rank")] int rank,
        [JsonProperty(PropertyName = "extremes_rank")] int extremesRank,
        [JsonProperty(PropertyName = "clan")] ClanData clan,
        [JsonProperty(PropertyName = "level_points")] int levelPoints,
        [JsonProperty(PropertyName = "members_count")] int memberCount,
        [JsonProperty(PropertyName = "hardest")] HardestInfo hardest,
        [JsonProperty(PropertyName = "extremes")] int extremes) {
        Rank = rank;
        ExtremesRank = extremesRank;
        Clan = clan;
        LevelPoints = levelPoints;
        MemberCount = memberCount;
        Hardest = hardest;
        Extremes = extremes;
    }
}