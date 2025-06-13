using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL.Data;

public class RecordLevelInfo {
    public string id;
    public string name;
    public string position;


    [JsonConstructor]
    public RecordLevelInfo(
        string id,
        string name,
        string position) {
        this.id = id;
        this.name = name;
        this.position = position;
    }
}