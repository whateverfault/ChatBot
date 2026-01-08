using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class ClanInfo {
    public readonly int Rank;
    public readonly int ExtremesRank;
    public readonly ClanData Clan;
    public readonly int LevelPoints;
    public readonly int MemberCount;
    public readonly HardestInfo Hardest;
    public readonly int Extremes;

    
    public ClanInfo(
        [JsonProperty("rank")] int rank,
        [JsonProperty("extremes_rank")] int extremesRank,
        [JsonProperty("clan")] ClanData clan,
        [JsonProperty("level_points")] int levelPoints,
        [JsonProperty("members_count")] int memberCount,
        [JsonProperty("hardest")] HardestInfo hardest,
        [JsonProperty("extremes")] int extremes) {
        Rank = rank;
        ExtremesRank = extremesRank;
        Clan = clan;
        LevelPoints = levelPoints;
        MemberCount = memberCount;
        Hardest = hardest;
        Extremes = extremes;
    }
}