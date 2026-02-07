using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class UserProfile {
    public readonly int Rank;
    public readonly int ExtremesRank;
    public readonly int RawRank;
    public readonly int CountryRank;
    public readonly int CountryExtremesRank;
    public readonly int CountryRawRank;
    public readonly UserInfo User;
    public readonly int? Country;
    public readonly int TotalPoints;
    public readonly int PackPoints;
    public readonly HardestInfo? Hardest;
    public readonly int Extremes;

    
    [JsonConstructor]
    public UserProfile(
        [JsonProperty("extremes")] int extremes,
        [JsonProperty("hardest")] HardestInfo? hardest,
        [JsonProperty("pack_points")] int packPoints,
        [JsonProperty("total_points")] int totalPoints,
        [JsonProperty("country")] int? country,
        [JsonProperty("user")] UserInfo user,
        [JsonProperty("country_raw_rank")] int countryRawRank,
        [JsonProperty("country_extremes_rank")] int countryExtremesRank,
        [JsonProperty("country_rank")] int countryRank,
        [JsonProperty("raw_rank")] int rawRank,
        [JsonProperty("extremes_rank")] int extremesRank,
        [JsonProperty("rank")] int rank) {
        Extremes = extremes;
        Hardest = hardest;
        PackPoints = packPoints;
        TotalPoints = totalPoints;
        Country = country;
        User = user;
        CountryRawRank = countryRawRank;
        CountryExtremesRank = countryExtremesRank;
        CountryRank = countryRank;
        RawRank = rawRank;
        ExtremesRank = extremesRank;
        Rank = rank;
    }
}