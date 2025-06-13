using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL.Data;

public class ClanInfo {
    public int rank;
    public int extremesRank;
    public ClanData clan;
    public int levelPoints;
    public int memberCount;
    public HardestInfo hardest;
    public int extremes;

    
    public ClanInfo(
        [JsonProperty(PropertyName = "rank")] int rank,
        [JsonProperty(PropertyName = "extremes_rank")] int extremesRank,
        [JsonProperty(PropertyName = "clan")] ClanData clan,
        [JsonProperty(PropertyName = "level_points")] int levelPoints,
        [JsonProperty(PropertyName = "members_count")] int memberCount,
        [JsonProperty(PropertyName = "hardest")] HardestInfo hardest,
        [JsonProperty(PropertyName = "extremes")] int extremes) {
        this.rank = rank;
        this.extremesRank = extremesRank;
        this.clan = clan;
        this.levelPoints = levelPoints;
        this.memberCount = memberCount;
        this.hardest = hardest;
        this.extremes = extremes;
    }
}