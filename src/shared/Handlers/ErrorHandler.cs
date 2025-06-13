using ChatBot.Services.logger;
using ChatBot.Services.Static;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.shared.Handlers;

public enum ErrorCode {
    PermDeny = 0,
    InvalidInput = 1,
    TooFewArgs = 2,
    AlreadyContains = 3,
    TooFewPoints = 4,
    ServiceDisabled = 5,
    AlreadyInState = 6,
    LogInIssue = 7,
    NotLoggedIn = 8,
    InvalidData = 9,
    NotInitialized = 10,
    SaveIssue,
    NotExist,
    ListIsEmpty,
    NotEnoughData,
    SmthWentWrong,
    ClipCreationFailed,
    None,
}

public class ErrorHandler {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    private static readonly string[] _internalErrorMessages = [
                                                                  "Permission Denied.",
                                                                  "Invalid Input.",
                                                                  "Too Few Arguments.",
                                                                  "List already contains such element.",
                                                                  "Not Enough Points.",
                                                                  "Function is Disabled.",
                                                                  "Already in Requested State.",
                                                                  "Something went wrong while reading saved info. Try to login first.",
                                                                  "Log in the bot first.",
                                                                  "Invalid data.",
                                                                  "Initialize the bot first.",
                                                                  "Something went wrong while reading saved info. Try to delete or rename save files.",
                                                                  "Doesn't exist.",
                                                                  "Empty.",
                                                                  "Too Few Data.",
                                                                  "Something Went Wrong.",
                                                                  "Failed to create a clip",
                                                              ];


    private static readonly string[] _twitchErrorMessages = [
                                                                "Недостаточно прав.",
                                                                "Неправильный ввод.",
                                                                "Слишком мало аругментов.",
                                                                "Такой элемент уже существует.",
                                                                "Недостаточно очков.",
                                                                "Функция отключена.",
                                                                "Уже находится в этом состоянии.",
                                                                "Что-то пошло не так при чтении сохраненных данных. Попробуйте сначала залогиниться.",
                                                                "Сначала залогиньтесь.",
                                                                "Некорректные данные.",
                                                                "Сначала инициализируйте бота.",
                                                                "Что-то пошло не так при чтении сохраненных данных. Попробуйте удалить или переименовать файлы.",
                                                                "Не существует.",
                                                                "Пусто.",
                                                                "Слишком мало данных.",
                                                                "Что-то пошло не так.",
                                                                "Не удалось создать клип",
                                                            ];


    private readonly ITwitchClient? _client;


    public ErrorHandler(ITwitchClient? client) {
        _client = client;
    }

    public bool ReplyWithError(ErrorCode code, ChatMessage message) {
        if (code == ErrorCode.None) {
            return false;
        }
        _logger.Log(LogLevel.Error, _internalErrorMessages[(int)code]);
        _client?.SendReply(message.Channel, message.Id, _twitchErrorMessages[(int)code]);
        return true;
    }

    public static bool SendError(ErrorCode code, ITwitchClient client, string channel) {
        if (code == ErrorCode.None) {
            return false;
        }
        _logger.Log(LogLevel.Error, _internalErrorMessages[(int)code]);
        client.SendMessage(channel, _twitchErrorMessages[(int)code]);
        return true;
    }
    
    public static bool ReplyWithError(ErrorCode code, ChatMessage message, ITwitchClient? client) {
        if (code == ErrorCode.None) {
            return false;
        }
        LogError(code);
        client?.SendReply(message.Channel, message.Id, _twitchErrorMessages[(int)code]);
        return true;
    }

    public static bool LogError(ErrorCode code) {
        if (code == ErrorCode.None) {
            return false;
        }
        _logger.Log(LogLevel.Error, _internalErrorMessages[(int)code]);
        return true;
    }

    public static bool LogErrorAndPrint(ErrorCode code) {
        var result = LogError(code);
        if (!result) {
            return result;
        }

        Console.WriteLine(_internalErrorMessages[(int)code]);
        Console.ReadKey();
        return result;
    }
}