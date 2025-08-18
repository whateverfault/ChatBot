using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class LevelInfo {
    public string id;
    public string name;
    public int position;
    public int points;
    public bool legacy;
    public int levelId;
    public bool twoPlayer;
    public string[]? tags;
    public string? description;
    public int? song;
    public float? edelEnjoyment;
    public bool isEdelPending;
    public float? gddlTier;
    public string? nlwTier;
    
    
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
        this.id = id;
        this.name = name;
        this.position = position;
        this.points = points;
        this.legacy = legacy;
        this.levelId = levelId;
        this.twoPlayer = twoPlayer;
        this.tags = tags;
        this.description = description;
        this.song = song;
        this.edelEnjoyment = edelEnjoyment;
        this.isEdelPending = isEdelPending;
        this.gddlTier = gddlTier;
        this.nlwTier = nlwTier;
    }
}