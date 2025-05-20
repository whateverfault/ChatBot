using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Shared.Handlers;

public enum ErrorCode {
    PermDeny = 0,
    WrongInput = 1,
    TooFewArgs = 2,
    AlreadyContains = 3,
    TooFewPoints = 4,
    ServiceDisabled = 5,
    AlreadyInState  = 6,
    LogInIssue = 7,
    NotLoggedIn = 8,
    InvalidData = 9,
    NotInitialized = 10,
    None,
}

public class ErrorHandler {
    private static readonly string[] _errorMessages = [
                                                          "Permission Denied.",
                                                          "Invalid Input.",
                                                          "Too Few Arguments.",
                                                          "List already contains such element.",
                                                          "Not Enough Points.",
                                                          "Function is Disabled.",
                                                          "Already in Requested State.",
                                                          "Something went wrong while logging in. Try to delete or rename the save file",
                                                          "Log in the bot first.",
                                                          "Invalid data.",
                                                          "Initialize the bot first..",
                                                      ];


    private readonly ITwitchClient _client;
    
    
    public ErrorHandler(ITwitchClient client) {
        _client = client;
    }
    
    public bool ReplyWithError(ErrorCode code, ChatMessage message) {
        if (code == ErrorCode.None) return false;
        Console.WriteLine($"[ErrorHandler] Caught an error: {code}");
        _client.SendReply(message.Channel, message.Id, _errorMessages[(int)code]);
        return true;
    }
    
    public static bool ReplyWithError(ErrorCode code, ChatMessage message, ITwitchClient client) {
        if (code == ErrorCode.None) return false;
        client.SendReply(message.Channel, message.Id, _errorMessages[(int)code]);
        return true;
    }

    public static bool LogError(ErrorCode code) {
        if (code == ErrorCode.None) return false;
        Console.WriteLine(_errorMessages[(int)code]);
        return true;
    }

    public static bool LogErrorAndWait(ErrorCode code) {
        var result = LogError(code);
        if (!result) return result;

        Console.ReadKey();
        return result;
    }
}