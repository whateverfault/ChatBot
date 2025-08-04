using TwitchLib.Client.Models;

namespace ChatBot.services.chat_commands.Parser.Data;

public class ParsedChatCommand {
    public char CommandIdentifier { get; }
    public string CommandText { get; }
    public string CommandMessage { get; }
    public List<string> ArgumentsAsList { get; }
    public ChatMessage ChatMessage { get; }


    public ParsedChatCommand(
        char commandIdentifier,
        string commandText,
        string commandMessage,
        List<string> argumentsAsList,
        ChatMessage chatMessage) {
        CommandIdentifier = commandIdentifier;
        CommandText = commandText;
        CommandMessage = commandMessage;
        ArgumentsAsList = argumentsAsList;
        ChatMessage = chatMessage;
    }
}