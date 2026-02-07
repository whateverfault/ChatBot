using ChatBot.api.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.aredl.responses;

public class ClanRecordsResponse {
    public readonly List<ClanSubmissionInfo?> Records;

    
    [JsonConstructor]
    public ClanRecordsResponse(
        [JsonProperty("records")] List<ClanSubmissionInfo?> records){
        Records = records;
    }
}