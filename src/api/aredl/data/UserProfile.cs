using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class UserProfile {
    public int? Rank;
    public int? ExtremesRank;
    public int? RawRank;
    public int? CountryRank;
    public int? CountryExtremesRank;
    public int? CountryRawRank;
    public UserInfo? User;
    public int? Country;
    public int? TotalPoints;
    public int? PackPoints;
    public HardestInfo? Hardest;
    public int? Extremes;

    
    [JsonConstructor]
    public UserProfile(
        [JsonProperty(PropertyName = "extremes")] int? extremes,
        [JsonProperty(PropertyName = "hardest")] HardestInfo? hardest,
        [JsonProperty(PropertyName = "pack_points")] int? packPoints,
        [JsonProperty(PropertyName = "total_points")] int? totalPoints,
        [JsonProperty(PropertyName = "country")] int? country,
        [JsonProperty(PropertyName = "user")] UserInfo? user,
        [JsonProperty(PropertyName = "country_raw_rank")] int? countryRawRank,
        [JsonProperty(PropertyName = "country_extremes_rank")] int? countryExtremesRank,
        [JsonProperty(PropertyName = "country_rank")] int? countryRank,
        [JsonProperty(PropertyName = "raw_rank")] int? rawRank,
        [JsonProperty(PropertyName = "extremes_rank")] int? extremesRank,
        [JsonProperty(PropertyName = "rank")] int? rank) {
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