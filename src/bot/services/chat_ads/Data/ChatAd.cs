using ChatBot.bot.interfaces;
using ChatBot.bot.services.Static;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_ads.Data;

public class ChatAd {
    private static readonly ChatAdsOptions _options = ((ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds)).Options;
    
    [JsonProperty("state")]
    private State State { get; set; }
    
    [JsonProperty("name")]
    private string Name { get; set; }
    
    [JsonProperty("output")]
    private string Output { get; set; }
    
    [JsonProperty("cooldown")]
    private long Cooldown { get; set; }
    
    [JsonProperty("threshold")]
    private int MessageThreshold { get; set; }

    [JsonProperty("last_sent_message_count")]
    private int LastSentMessageCount { get; set; }
    
    [JsonProperty("last_sent")]
    private long LastSent { get; set; }
    
    
    public ChatAd(
        string name = "--",
        string output = "--",
        long cooldown = 1800,
        int messageThreshold = 0) {
        State = State.Enabled;
        Name = name;
        Output = output;
        Cooldown = cooldown <= 0? 1800 : cooldown;
        MessageThreshold = messageThreshold;
        
        LastSentMessageCount = 0;
        LastSent = 0;
    }

    [JsonConstructor]
    public ChatAd(
        [JsonProperty("state")] State state,
        [JsonProperty("name")] string name,
        [JsonProperty("output")] string output,
        [JsonProperty("cooldown")] long cooldown,
        [JsonProperty("threshold")] int messageThreshold,
        [JsonProperty("last_sent_message_count")] int lastSentMessageCount,
        [JsonProperty("last_sent")] long lastSent) {
        State = state;
        Name = name;
        Output = output;
        Cooldown = cooldown;
        MessageThreshold = messageThreshold;
        LastSentMessageCount = lastSentMessageCount;
        LastSent = lastSent;
    }
    
    public void StateNext() {
        State = (State)(((int)State+1)%Enum.GetValues(typeof(State)).Length);
        _options.Save();
    }

    public void SetName(string name) {
        Name = name;
        _options.Save();
    }

    public void SetOutput(string output) {
        Output = output;
        _options.Save();
    }

    public void SetCooldown(long cooldown) {
        Cooldown = cooldown;
        _options.Save();
    }

    public void SetThreshold(int threshold) {
        MessageThreshold = threshold;
    }
    
    public void SetLastSentMessageCount(int messageCount) {
        LastSentMessageCount = messageCount;
    }
    
    public void SetLastSentTime() {
        LastSent = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _options.Save();
    }

    public State GetState() {
        return State;
    }

    public int GetStateAsInt() {
        return (int)State;
    }
    
    public string GetName() {
        return Name;
    }

    public string GetOutput() {
        return Output;
    }

    public long GetCooldown() {
        return Cooldown;
    }

    public int GetThreshold() {
        return MessageThreshold;
    }

    public int GetLastSentMessageCount() {
        return LastSentMessageCount;
    }
    
    public long GetLastTimeSent() {
        return LastSent;
    }
}