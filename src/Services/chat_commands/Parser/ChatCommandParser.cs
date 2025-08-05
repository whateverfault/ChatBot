using System.Text;
using ChatBot.services.chat_commands.Parser.Data;
using TwitchLib.Client.Models;

namespace ChatBot.services.chat_commands.Parser;

public static class ChatCommandParser {
    public static ParsedChatCommand? Parse(ChatMessage chatMessage) {
        var commandIdentifier = ' ';

        var commandMessage = chatMessage.Message
                                        .Replace($"{(char)56128}", "")
                                        .Replace($"{(char)56320}", "")
                                        .Trim();

        if (commandMessage.Length < 1) {
            return null;
        }

        if (char.IsPunctuation(commandMessage[0])) {
            commandIdentifier = commandMessage[0];
        }

        var startIndex =
            commandIdentifier == ' ' ? 0 : 1;
        
        if (commandMessage.Length <= startIndex) {
            return null;
        }

        var commandText = GetCommandText(startIndex, commandMessage, out var end);

        if (string.IsNullOrEmpty(commandText)) {
            return null;
        }

        commandMessage = GetCommandMessage(end, commandMessage).Trim();
        var argumentsAsList = commandMessage.Split(" ", StringSplitOptions.TrimEntries).ToList();
        argumentsAsList.RemoveAll(x => x.Equals(""));
        
        return new ParsedChatCommand(
                                     commandIdentifier, 
                                     commandText,
                                     commandMessage,
                                     argumentsAsList,
                                     chatMessage
                                     );
    }

    private static string GetCommandText(int start, string message, out int end) {
        var sb = new StringBuilder();
        var index = start;

        for (; index < message.Length; ++index) {
            if (message[index] == ' ') {
                break;
            }

            sb.Append(message[index]);
        }

        end = index;
        return sb.ToString();
    }
    
    private static string GetCommandMessage(int start, string message) {
        var sb = new StringBuilder();

        for (var i = start; i < message.Length; ++i) {
            sb.Append(message[i]);
        }

        return sb.ToString();
    }
}