using ChatBot.api.demon_list.data;
using ChatBot.api.demon_list.glist.data;

namespace ChatBot.api.demon_list.glist;

public class GListCache {
    public List<LevelInfo>? Levels { get; private set; }
    
    public void CacheLevelsList(List<LevelInfo>? levels) {
        Levels = levels;
    }
    
    public void ResetCache() {
        Levels = null;
    }
}