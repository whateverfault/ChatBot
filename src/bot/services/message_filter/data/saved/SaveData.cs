using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.message_filter.data.saved;

internal class SaveData {
    [JsonProperty("state")]
    public State State { get; set; }
    
    [JsonProperty("filters")]
    public List<Filter> Filters { get; private set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("filters")] List<Filter> filters,
        [JsonProperty("state")] State state) {
        var dto = new SaveDataDto(
                                  filters,
                                  state
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        Filters = dto.Filters.Value;
        State = dto.State.Value;
    }
}