using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class LevelInfo {
    [JsonProperty("id")]
    public string Id { get; private set; }
    
    [JsonProperty("name")]
    public string Name { get; private set; }
    
    [JsonProperty("position")]
    public int Position { get; private set; }
    
    [JsonProperty("points")]
    public int Points { get; private set; }
    
    [JsonProperty("legacy")]
    public bool Legacy { get; private set; }
    
    [JsonProperty("level_id")]
    public int LevelId { get; private set; }
    
    [JsonProperty("two_player")]
    public bool TwoPlayer { get; private set; }
    
    [JsonProperty("tags")]
    public string[]? Tags { get; private set; }
    
    [JsonProperty("description")]
    public string? Description { get; private set; }
    
    [JsonProperty("song")]
    public int? Song { get; private set; }
    
    [JsonProperty("edel_enjoyment")]
    public float? EdelEnjoyment { get; private set; }
    
    [JsonProperty("is_edel_pending")]
    public bool IsEdelPending { get; private set; }
    
    [JsonProperty("gddl_tier")]
    public float? GddlTier { get; private set; }
    
    [JsonProperty("nlw_tier")]
    public string? NlwTier { get; private set; }
    
    
    [JsonConstructor]
    public LevelInfo(
        [JsonProperty("id")] string id,
        [JsonProperty("name")] string name,
        [JsonProperty("position")] int position,
        [JsonProperty("points")] int points,
        [JsonProperty("legacy")] bool legacy,
        [JsonProperty("level_id")] int levelId,
        [JsonProperty("two_player")] bool twoPlayer,
        [JsonProperty("tags")] string[]? tags,
        [JsonProperty("description")] string? description,
        [JsonProperty("song")] int? song,
        [JsonProperty("edel_enjoyment")] float? edelEnjoyment,
        [JsonProperty("is_edel_pending")] bool isEdelPending,
        [JsonProperty("gddl_tier")] float? gddlTier,
        [JsonProperty("nlw_tier")] string? nlwTier) {
        Id = id;
        Name = name;
        Position = position;
        Points = points;
        Legacy = legacy;
        LevelId = levelId;
        TwoPlayer = twoPlayer;
        Tags = tags;
        Description = description;
        Song = song;
        EdelEnjoyment = edelEnjoyment;
        IsEdelPending = isEdelPending;
        GddlTier = gddlTier;
        NlwTier = nlwTier;
    }
}