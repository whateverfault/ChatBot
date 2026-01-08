using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.cli.data.rendering.saved;

internal class SaveData {
    [JsonProperty("current_renderer")]
    public Renderer CurrentRenderer { get; set; }


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] Renderer curRenderer) {
        var dto = new SaveDataDto(curRenderer);
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        CurrentRenderer = dto.CurrentRenderer.Value;
    }
}