using ChatBot.utils.GD.AREDL.Data;

namespace ChatBot.utils.GD.AREDL.Responses;

public class ListLevelsResponse {
    public readonly List<LevelInfo>? data;


    public ListLevelsResponse(List<LevelInfo>? data) {
        this.data = data;
    }
}