using ChatBot.bot.shared.handlers;
using ChatBot.bot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_commands.Data;

public sealed class CustomChatCommand : ChatCommand {
    [JsonProperty("id")]
    public override int Id { get; protected set; }
    [JsonProperty("name")]
    public override string Name { get; protected set; }
    [JsonProperty("args")]
    public override string Args { get; protected set; }
    [JsonProperty("description")]
    public override string Description { get; protected set; }
    [JsonProperty("has_identifier")]
    public override bool HasIdentifier { get; protected set; }
    [JsonProperty("aliases")]
    public override List<string>? Aliases { get; protected set; }
    [JsonProperty("output")]
    public string Output { get; private set; }
    [JsonProperty("cooldown")]
    public override int Cooldown { get; protected set; }
    [JsonIgnore]
    public override long LastUsed { get; protected set; }
    [JsonIgnore]
    public override CmdActionHandler Action { get; protected set; }
    [JsonProperty("restriction")]
    public override Restriction Restriction { get; protected set; }
    [JsonProperty("state")]
    public override State State { get; protected set; }
    
    
    public CustomChatCommand(
        int id,
        string name,
        string args,
        string description,
        bool hasIdentifier,
        List<string>? aliases,
        string output,
        Restriction restriction,
        int cooldown = 1) {
        Id = id;
        Name = name.Replace(" ", "");
        Args = args;
        Description = description;
        HasIdentifier = hasIdentifier;
        Aliases = aliases;
        Output = output;
        Action = CmdAction;
        Restriction = restriction;
        Cooldown = cooldown;
        LastUsed = 0;
        State = State.Enabled;
    }
    
    [JsonConstructor]
    public CustomChatCommand(
        [JsonProperty(PropertyName = "id")] int id,
        [JsonProperty(PropertyName = "name")] string name,
        [JsonProperty(PropertyName = "args")] string args,
        [JsonProperty(PropertyName = "description")] string description,
        [JsonProperty(PropertyName = "aliases")] List<string>? aliases,
        [JsonProperty(PropertyName = "output")] string output,
        [JsonProperty(PropertyName = "restriction")] Restriction restriction,
        [JsonProperty(PropertyName = "state")] State state,
        [JsonProperty(PropertyName = "cooldown")] int cooldown) {
        Id = id;
        Name = name.Replace(" ", "");
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
        ChatCommandsService.Options.Save();
    }
    
    private Task CmdAction(ChatCmdArgs chatArgs) {
        var command = (CustomChatCommand)chatArgs.Command;
        var chatMessage = chatArgs.Parsed.ChatMessage;
        var client = TwitchChatBot.Instance.GetClient();

        if (HasIdentifier) {
            client?.SendReply(chatMessage.Id, command.Output);
        }
        else {
            if (chatArgs.Parsed.ArgumentsAsList.Count != 0) return Task.CompletedTask;
            client?.SendMessage(command.Output);
        }
        return Task.CompletedTask;
    }
}