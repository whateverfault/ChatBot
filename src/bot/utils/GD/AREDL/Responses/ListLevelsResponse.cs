using ChatBot.bot.utils.GD.AREDL.Data;

namespace ChatBot.bot.utils.GD.AREDL.Responses;

public class ListLevelsResponse {
    public readonly List<LevelInfo>? data;


    public ListLevelsResponse(List<LevelInfo>? data) {
        this.data = data;
    }
}