using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class RankData {
    [JsonProperty("rank")]
    public int Rank { get; private set; }
    
    [JsonProperty("raw_rank")]
    public int RawRank { get; private set; }
    
    [JsonProperty("extremes_rank")]
    public int ExtremesRank { get; private set; }

    [JsonProperty("country_rank")]
    public int CountryRank { get; private set; }
    
    [JsonProperty("total_points")]
    public int TotalPoints { get; private set; }
    
    [JsonProperty("extremes")]
    public int Extremes { get; private set; }
    
    
    public RankData(
        [JsonProperty("rank")] int rank,
        [JsonProperty("raw_rank")] int rawRank,
        [JsonProperty("extremes_rank")] int extremesRank,
        [JsonProperty("country_rank")] int countryRank,
        [JsonProperty("total_points")] int totalPoints,
        [JsonProperty("extremes")] int extremes) {
        Rank = rank;
        RawRank = rawRank;
        ExtremesRank = extremesRank;
        CountryRank = countryRank;
        TotalPoints = totalPoints;
        Extremes = extremes;
    }
}