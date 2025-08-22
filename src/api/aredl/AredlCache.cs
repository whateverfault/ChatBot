using ChatBot.api.aredl.responses;

namespace ChatBot.api.aredl;

public class AredlCache {
    public ListLevelsResponse? Levels { get; private set; }
    public ListLevelsResponse? PlatformerLevels { get; private set; }
    public ListClansResponse? Clans { get; private set; }


    public void CacheLevelsList(ListLevelsResponse levels) {
        Levels = levels;
    }

    public void CachePlatformerLevelsList(ListLevelsResponse platformerLevels) {
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