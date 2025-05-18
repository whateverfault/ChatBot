using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot;

public enum ErrorCode {
    PermDeny,
    WrongInput,
    TooFewArgs,
    AlreadyContains,
    TooFewPoints,
    ServiceDisabled,
    None,
}

public class ErrorHandler {
    private static readonly string[] _errorMessages = [
                                                          "Недостаточно прав.",
                                                          "Неправильный ввод.",
                                                          "Слишком мало аргументов.",
                                                          "Такой эллемент уже существует.",
                                                          "У вас недостаточно очков.",
                                                          "Стример отключил эту функцию.",
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
}