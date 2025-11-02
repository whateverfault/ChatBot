using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.shared.handlers;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_commands.data;

public sealed class CustomChatCommand : ChatCommand {
    [JsonProperty("output")]
    public string Output { get; private set; }
    
    
    public CustomChatCommand(
        int id,
        string name,
        string args,
        string description,
        bool hasIdentifier,
        List<string> aliases,
        string output,
        Restriction restriction,
        int cooldown = 1,
        bool cooldownPerUser = true) : base(
                                  id,
                                  name,
                                  args,
                                  description,
                                  hasIdentifier,
                                  aliases,
                                  cooldown,
                                  cooldownPerUser,
                                  [],
                                  0,
                                  CmdAction,
                                  restriction,
                                  State.Enabled){
        Output = output;
    }
    
    [JsonConstructor]
    public CustomChatCommand(
        [JsonProperty("id")] int id,
        [JsonProperty("name")] string name,
        [JsonProperty("args")] string args,
        [JsonProperty("description")] string description,
        [JsonProperty("has_identifier")] bool hasIdentifier,
        [JsonProperty("aliases")] List<string> aliases,
        [JsonProperty("cooldown")] int cooldown,
        [JsonProperty("cooldown_per_user")] bool cooldownPerUser,
        [JsonProperty("cooldown_users")] List<CooldownUser> cooldownUsers,
        [JsonProperty("last_used")] long lastUsed,
        [JsonProperty("output")] string output,
        [JsonProperty("restriction")] Restriction restriction,
        [JsonProperty("state")] State state) : base(
                                                      id,
                                                      name,
                                                      args,
                                                      description,
                                                      hasIdentifier,
                                                      aliases,
                                                      cooldown,
                                                      cooldownPerUser,
                                                      cooldownUsers,
                                                      lastUsed,
                                                      CmdAction,
                                                      restriction,
                                                      state) {
        Output = output;
    }

    public string GetOutput() {
        return Output;
    }

    public void SetOutput(string output) {
        Output = output;
        ChatCommandsService.Options.Save();
    }
    
    private static Task CmdAction(ChatCmdArgs chatArgs) {
        var client = TwitchChatBot.Instance.GetClient();
        if (client == null) return Task.CompletedTask;
        
        var command = (CustomChatCommand)chatArgs.Command;
        var chatMessage = chatArgs.Parsed.ChatMessage;
        
        if (command.HasIdentifier) {
            client.SendMessage(command.Output, chatMessage.Id);
        }else {
            client.SendMessage(command.Output);
        }
        return Task.CompletedTask;
    }
}