using ChatBot.api.aredl.data;
using ChatBot.api.aredl.responses;

namespace ChatBot.api.aredl;

public class AredlCache {
    public List<LevelInfo>? Levels { get; private set; }
    public List<LevelInfo>? PlatformerLevels { get; private set; }
    public ListClansResponse? Clans { get; private set; }


    public void CacheLevelsList(List<LevelInfo> levels) {
        Levels = levels;
    }

    public void CachePlatformerLevelsList(List<LevelInfo> platformerLevels) {
        PlatformerLevels = platformerLevels;
    }

    public void CacheClansList(ListClansResponse clans) {
        Clans = clans;
    }

    public void ResetCache() {
        Levels = null;
        PlatformerLevels = null;
        Clans = null;
    }
}