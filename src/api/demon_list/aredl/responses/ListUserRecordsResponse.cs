using ChatBot.api.demon_list.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.demon_list.aredl.responses;

public class ListUserRecordsResponse {
    public readonly List<RecordInfo> Records;


    public ListUserRecordsResponse(
        [JsonProperty("records")] List<RecordInfo>? records) {
        Records = records ?? [];
    }
}