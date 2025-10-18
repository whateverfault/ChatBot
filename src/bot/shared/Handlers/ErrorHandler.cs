using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using TwitchAPI.client;
using TwitchAPI.client.data;

namespace ChatBot.bot.shared.handlers;

public enum ErrorCode {
    InvalidInput = 0,
    TooFewArgs,
    AlreadyExists,
    ServiceDisabled,
    CorruptedCredentials,
    InvalidData,
    NotInitialized,
    Empty,
    NotEnoughData,
    SmthWentWrong,
    ClipCreationFailed,
    RequestFailed,
    TranslationFailed,
    ConnectionFailed,
    NoRewardSet,
    UserNotFound,
    TooFewPoints,
    AccountNotFound,
    NoAvailable,
    NotFound,
    NotEnough,
    PermissionDenied,
    None,
}

public static class ErrorHandler {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    private static readonly string[] _internalErrorMessages = [
                                                                  "Invalid Input.",
                                                                  "Too Few Arguments.",
                                                                  "Already exists.",
                                                                  "Function is Disabled.",
                                                                  "Corrupted credentials.",
                                                                  "Invalid data.",
                                                                  "Not Initialized.",
                                                                  "List is Empty.",
                                                                  "Too Few Data.",
                                                                  "Something Went Wrong.",
                                                                  "Failed to create a clip",
                                                                  "Request Failed.",
                                                                  "Translation failed.",
                                                                  "Connection failed.",
                                                                  "Reward isn't properly set.",
                                                                  "User not found.",
                                                                  "Too few points.",
                                                                  "Account not found.",
                                                                  "No Available.",
                                                                  "Not Found.",
                                                                  "Not Enough.",
                                                                  "Permission denied.",
                                                              ];


    private static readonly string[] _twitchErrorMessages = [
                                                                "Неправильный ввод.",
                                                                "Слишком мало аругментов.",
                                                                "Уже существует.",
                                                                "Функция отключена.",
                                                                "Некорректные данные для подключения.",
                                                                "Некорректные данные.",
                                                                "Не инициализированно.",
                                                                "Пусто.",
                                                                "Слишком мало данных.",
                                                                "Что-то пошло не так.",
                                                                "Не удалось создать клип.",
                                                                "Запрос не удался.",
                                                                "Не удалось перевести.",
                                                                "Не удалось подключиться к сети.",
                                                                "Награда не установлена.",
                                                                "Пользователь не найден.",
                                                                "Недостаточно фантиков.",
                                                                "Аккаунт не найден.",
                                                                "Нет доступных.",
                                                                "Не найдено.",
                                                                "Недостаточно.",
                                                                "Недостаточно прав.",
                                                            ];


    public static string? GetErrorString(ErrorCode? error, bool eng = false) {
        if (error == null) return null;
        
        if (eng) return _internalErrorMessages[(int)error];
        return _twitchErrorMessages[(int)error];
    }
    
    public static async Task<bool> ReplyWithError(ErrorCode? error, ChatMessage message, ITwitchClient? client) {
        if (client == null || error == null || error == ErrorCode.None) {
            return false;
        }
        
        await client.SendMessage($"Ошибка: {_twitchErrorMessages[(int)error]}", message.Id);
        return true;
    }

    public static void LogMessage(LogLevel logLevel, string message) {
        _logger.Log(logLevel, message);
    }
    
    public static bool LogError(ErrorCode code) {
        if (code == ErrorCode.None) {
            return false;
        }
        
        LogMessage(LogLevel.Error, _internalErrorMessages[(int)code]);
        return true;
    }

    public static bool LogErrorAndPrint(ErrorCode error) {
        var result = LogError(error);
        if (!result) {
            return result;
        }

        Console.Clear();
        Console.WriteLine(_internalErrorMessages[(int)error]);
        Console.ReadLine();
        Console.Clear();
        return result;
    }
    
    public static void LogErrorMessageAndPrint(ErrorCode error, string message) {
        LogError(error);
        
        Console.Clear();
        Console.WriteLine(message);
        Console.ReadKey();
        Console.Clear();
    }
    
    public static void PrintMessage(string message) {
        Console.Clear();
        Console.WriteLine(message);
        Console.ReadKey();
        Console.Clear();
    }
}