using ChatBot.api.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.aredl.responses;

public class ListUserRecordsResponse {
    public List<RecordInfo> records;
    public List<RecordInfo> verified;


    public ListUserRecordsResponse(
        [JsonProperty(PropertyName = "records")] List<RecordInfo> records,
        [JsonProperty(PropertyName = "verified")] List<RecordInfo> verified) {
        this.records = records;
        this.verified = verified;
    }
}