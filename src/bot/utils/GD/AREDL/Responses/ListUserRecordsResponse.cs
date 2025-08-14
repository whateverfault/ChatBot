using ChatBot.bot.utils.GD.AREDL.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.utils.GD.AREDL.Responses;

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