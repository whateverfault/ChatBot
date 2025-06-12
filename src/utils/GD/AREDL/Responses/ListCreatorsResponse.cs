using ChatBot.utils.GD.AREDL.Data;

namespace ChatBot.utils.GD.AREDL.Responses;

public class ListCreatorsResponse {
    public List<LevelCreatorInfo>? data;

    
    public ListCreatorsResponse(List<LevelCreatorInfo>? data) {
        this.data = data;
    }
}