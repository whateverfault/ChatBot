using Newtonsoft.Json;

namespace ChatBot.bot.utils.GD.AREDL.Data;

public class RecordLevelInfo {
    public string id;
    public string name;
    public int position;
    public bool legacy;


    [JsonConstructor]
    public RecordLevelInfo(
        string id,
        string name,
        int position, 
        bool legacy) {
        this.id = id;
        this.name = name;
        this.position = position;
        this.legacy = legacy;
    }
}