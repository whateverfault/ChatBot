using ChatBot.api.demon_list.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.demon_list.aredl.responses;

public class ClanRecordsResponse {
    public readonly List<ClanSubmissionInfo?> Records;

    
    [JsonConstructor]
    public ClanRecordsResponse(
        [JsonProperty("records")] List<ClanSubmissionInfo?> records){
        Records = records;
    }
}