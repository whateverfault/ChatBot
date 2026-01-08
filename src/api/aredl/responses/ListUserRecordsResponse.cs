using ChatBot.api.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.aredl.responses;

public class ListUserRecordsResponse {
    public readonly List<RecordInfo> Records;
    public readonly List<RecordInfo> Verified;


    public ListUserRecordsResponse(
        [JsonProperty("records")] List<RecordInfo> records,
        [JsonProperty("verified")] List<RecordInfo> verified) {
        Records = records;
        Verified = verified;
    }
}