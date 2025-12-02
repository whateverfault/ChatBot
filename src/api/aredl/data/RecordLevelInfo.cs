using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class RecordLevelInfo {
    public readonly string Id;
    public readonly string Name;
    public readonly int Position;
    public readonly bool Legacy;


    [JsonConstructor]
    public RecordLevelInfo(
        string id,
        string name,
        int position, 
        bool legacy) {
        Id = id;
        Name = name;
        Position = position;
        Legacy = legacy;
    }
}