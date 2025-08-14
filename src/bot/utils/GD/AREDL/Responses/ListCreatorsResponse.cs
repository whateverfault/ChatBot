using ChatBot.bot.utils.GD.AREDL.Data;

namespace ChatBot.bot.utils.GD.AREDL.Responses;

public class ListCreatorsResponse {
    public List<LevelCreatorInfo>? data;

    
    public ListCreatorsResponse(List<LevelCreatorInfo>? data) {
        this.data = data;
    }
}