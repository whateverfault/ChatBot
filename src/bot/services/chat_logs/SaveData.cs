﻿using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_logs;

internal class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty(PropertyName = "logs")]
    public List<Message> Logs { get; private set; }


    public SaveData() {
        ServiceState = State.Enabled;
        Logs = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "logs")] List<Message> logs) {
        ServiceState = serviceState;
        Logs = logs;
    }
}