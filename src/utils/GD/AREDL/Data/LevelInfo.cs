using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL.Data;

public class LevelInfo {
    public string id;
    public string name;
    public int position;
    public string publisherId;
    public int points;
    public bool legacy;
    public int levelId;
    public bool twoPlayer;
    public string[]? tags;
    public string? description;
    public int? song;
    public int? edelEnjoyment;
    public bool isEdelPending;
    public int? gddlTier;
    public string? nlwTier;
    
    
    [JsonConstructor]
    public LevelInfo(
        string id,
        string name,
        int position,
        string publisherId,
        int points,
        bool legacy,
        int levelId,
        bool twoPlayer,
        string[]? tags,
        string? description,
        int? song,
        int? edelEnjoyment,
        bool isEdelPending,
        int? gddlTier,
        string? nlwTier) {
        this.id = id;
        this.name = name;
        this.position = position;
        this.publisherId = publisherId;
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