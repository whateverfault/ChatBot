using ChatBot.bot.interfaces;
using ChatBot.bot.shared.handlers;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_commands.data;

public sealed class DefaultChatCommand : ChatCommand {
    public DefaultChatCommand(
        int id,
        string name,
        string args,
        string description,
        CmdActionHandler action,
        Restriction restriction,
        int cooldown = 1, 
        List<string>? aliases = null,
        bool hasIdentifier = true) : base(
                                          id,
                                          name,
                                          args,
                                          description,
                                          hasIdentifier,
                                          aliases ?? [],
                                          cooldown,
                                          true,
                                          [],
                                          0,
                                          action,
                                          restriction,
                                          State.Enabled) {
    }
    
    [JsonConstructor]
    public DefaultChatCommand(
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
                                                    GetDefaultCmdActionHandler(id),
                                                    restriction,
                                                    State.Enabled) {
    }

    private static CmdActionHandler GetDefaultCmdActionHandler(int id) {
        foreach (var cmd in CommandsList.DefaultsCommands) {
            if (cmd.Id != id) continue;

            return cmd.Action;
        }
        
        return _ => Task.CompletedTask;
    }
}