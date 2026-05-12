using Newtonsoft.Json;

namespace ChatBot.api.demon_list.aredl.data;

public class ClanDetails {
    [JsonProperty("clan")]
    public ClanData Clan { get; private set; }
    
    [JsonProperty("rank")]
    public RankData? Rank { get; private set; }

    [JsonProperty("records")]
    public List<RecordInfo> Records;
    
    
    public ClanDetails(
        [JsonProperty("clan")] ClanData clan,
        [JsonProperty("rank")] RankData? rank,
        [JsonProperty("records")] List<RecordInfo> records) {
        Records = records;
        Clan = clan;
        Rank = rank;
    }
}