using ChatBot.api.aredl.data;

namespace ChatBot.api.aredl.responses;

public class ListCreatorsResponse {
    public List<LevelCreatorInfo>? data;

    
    public ListCreatorsResponse(List<LevelCreatorInfo>? data) {
        this.data = data;
    }
}