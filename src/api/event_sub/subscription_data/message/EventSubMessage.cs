using Newtonsoft.Json;

namespace ChatBot.api.event_sub.subscription_data.message;

public class EventSubMessage<TMetadata, TPayload>  {
    [JsonProperty("metadata")]
    public TMetadata Metadata { get; set; }

    [JsonProperty("payload")]
    public TPayload Payload { get; set; }
    
    
    [JsonConstructor]
    public EventSubMessage(
        [JsonProperty("metadata")] TMetadata metadata,
        [JsonProperty("payload")] TPayload payload) {
        Metadata = metadata;
        Payload = payload;
    }
}