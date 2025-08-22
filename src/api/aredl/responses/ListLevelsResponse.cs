using ChatBot.api.aredl.data;

namespace ChatBot.api.aredl.responses;

public class ListLevelsResponse {
    public List<LevelInfo> Data { get; private set; }

    
    public ListLevelsResponse(List<LevelInfo> data) {
        Data = data;
    }
}