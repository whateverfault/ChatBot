using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.chat_commands;

public sealed class CustomChatCommand : ChatCommand {
    [JsonProperty(PropertyName = "name")]
    public override string Name { get; protected set; }
    [JsonProperty(PropertyName = "args")]
    public override string Args { get; protected set; }
    [JsonProperty(PropertyName = "description")]
    public override string Description { get; protected set; }
    [JsonProperty(PropertyName = "aliases")]
    public override List<string>? Aliases { get; protected set; }
    [JsonProperty(PropertyName = "output")]
    public string Output { get; private set; }
    [JsonProperty(PropertyName = "cooldown")]
    public override int Cooldown { get; protected set; }
    [JsonIgnore]
    public override long LastUsed { get; protected set; }
    [JsonIgnore]
    public override CmdActionHandler? Action { get; protected set; }
    [JsonProperty(PropertyName = "restriction")]
    public override Restriction Restriction { get; protected set; }
    [JsonProperty(PropertyName = "state")]
    public override State State { get; protected set; }
    
    
    public CustomChatCommand(
        string name,
        string args,
        string description,
        List<string>? aliases,
        string output,
        Restriction restriction,
        State state,
        int cooldown,
        long lastUsed) {
        Name = name;
        Args = args;
        Description = description;
        Aliases = aliases;
        Output = output;
        Action = CmdAction;
        Restriction = restriction;
        State = state;
        Cooldown = cooldown;
        LastUsed = lastUsed;
    }
    
    public CustomChatCommand(
        [JsonProperty(PropertyName = "name")] string name,
        [JsonProperty(PropertyName = "args")] string args,
        [JsonProperty(PropertyName = "description")] string description,
        [JsonProperty(PropertyName = "aliases")] List<string>? aliases,
        [JsonProperty(PropertyName = "output")] string output,
        [JsonProperty(PropertyName = "restriction")] Restriction restriction,
        [JsonProperty(PropertyName = "state")] State state,
        [JsonProperty(PropertyName = "cooldown")] int cooldown) {
        Name = name;
        Args = args;
        Description = description;
        Aliases = aliases;
        Output = output;
        Action = CmdAction;
        Restriction = restriction;
        State = state;
        Cooldown = cooldown;
        LastUsed = 0;
    }

    public string GetOutput() {
        return Output;
    }

    public void SetOutput(string output) {
        Output = output;
    }
    
    private Task CmdAction(ChatCmdArgs chatArgs) {
        if (chatArgs.Command.GetType() != typeof(CustomChatCommand)) return Task.CompletedTask;

        var command = (CustomChatCommand)chatArgs.Command;
        var chatMessage = chatArgs.Args.Command.ChatMessage;
        var client = chatArgs.Bot.GetClient();
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, command.Output);
        return Task.CompletedTask;
    }
}