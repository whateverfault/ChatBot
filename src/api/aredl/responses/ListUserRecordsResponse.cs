using ChatBot.api.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.aredl.responses;

public class ListUserRecordsResponse {
    public readonly List<RecordInfo> Records;


    public ListUserRecordsResponse(
        [JsonProperty("records")] List<RecordInfo>? records) {
        Records = records ?? [];
    }
}