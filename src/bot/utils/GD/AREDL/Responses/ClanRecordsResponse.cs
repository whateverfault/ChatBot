using ChatBot.utils.GD.AREDL.Data;
using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL.Responses;

public class ClanRecordsResponse {
    public List<ClanSubmissionInfo?> records;
    public List<ClanSubmissionInfo?> verified;

    
    [JsonConstructor]
    public ClanRecordsResponse(
        [JsonProperty(PropertyName = "records")] List<ClanSubmissionInfo?> records,
        [JsonProperty(PropertyName = "verified")] List<ClanSubmissionInfo?> verified) {
        this.records = records;
        this.verified = verified;
    }
}