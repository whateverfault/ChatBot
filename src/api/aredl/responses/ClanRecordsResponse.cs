using ChatBot.api.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.aredl.responses;

public class ClanRecordsResponse {
    public readonly List<ClanSubmissionInfo?> Records;
    public readonly List<ClanSubmissionInfo?> Verified;

    
    [JsonConstructor]
    public ClanRecordsResponse(
        [JsonProperty("records")] List<ClanSubmissionInfo?> records,
        [JsonProperty("verified")] List<ClanSubmissionInfo?> verified) {
        Records = records;
        Verified = verified;
    }
}