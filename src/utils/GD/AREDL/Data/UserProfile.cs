using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL.Data;

public class UserProfile {
    public int? rank;
    public int? extremesRank;
    public int? rawRank;
    public int? countryRank;
    public int? countryExtremesRank;
    public int? countryRawRank;
    public UserInfo? user;
    public int? country;
    public int? totalPoints;
    public int? packPoints;
    public HardestInfo? hardest;
    public int? extremes;

    
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
        this.extremes = extremes;
        this.hardest = hardest;
        this.packPoints = packPoints;
        this.totalPoints = totalPoints;
        this.country = country;
        this.user = user;
        this.countryRawRank = countryRawRank;
        this.countryExtremesRank = countryExtremesRank;
        this.countryRank = countryRank;
        this.rawRank = rawRank;
        this.extremesRank = extremesRank;
        this.rank = rank;
    }
}