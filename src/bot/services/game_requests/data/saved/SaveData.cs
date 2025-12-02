using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.game_requests.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("game_requests_rewards")]
    public List<string> GameRequestsRewards { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("game_requests_rewards")] List<string> gameRequestsRewards) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  gameRequestsRewards
                                  );

        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        GameRequestsRewards = dto.GameRequestsRewards.Value;
    }
}