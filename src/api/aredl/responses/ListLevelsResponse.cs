using ChatBot.api.aredl.data;

namespace ChatBot.api.aredl.responses;

public class ListLevelsResponse {
    public readonly List<LevelInfo>? data;


    public ListLevelsResponse(List<LevelInfo>? data) {
        this.data = data;
    }
}