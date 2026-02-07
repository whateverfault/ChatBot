using ChatBot.bot.services.localization;
using ChatBot.bot.services.localization.data;
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
    TooFewAccounts,
    NoAvailable,
    NotFound,
    NotEnough,
    PermissionDenied,
    AntiMute,
    NotFullyAuthorized,
    None,
}

public static class ErrorHandler {
    private static readonly LoggerService _logger = (LoggerService)Services.Get(ServiceId.Logger);

    private static readonly LocalizationService _localization =
        (LocalizationService)Services.Get(ServiceId.Localization);

    private static readonly string[] _engErrorMessages = [
                                                             "Invalid Input.",
                                                             "Too Few Arguments.",
                                                             "Already exists.",
                                                             "Function is Disabled.",
                                                             "Your credentials got corrupted. You might want to update them.",
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
                                                             "Too Few Accounts.",
                                                             "No Available.",
                                                             "Not Found.",
                                                             "Not Enough.",
                                                             "Permission denied.",
                                                             "AntiMute Activated.",
                                                             "This Function Requires Full Authorization.",
                                                         ];


    private static readonly string[] _ruErrorMessages = [
                                                            "Неправильный ввод.",
                                                            "Слишком мало аругментов.",
                                                            "Уже существует.",
                                                            "Функция отключена.",
                                                            "Данные для подключения требуют обновления.",
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
                                                            "Слишком мало аккаунтов.",
                                                            "Нет доступных.",
                                                            "Не найдено.",
                                                            "Недостаточно.",
                                                            "Недостаточно прав.",
                                                            "Сработала защита от мута.",
                                                            "Эта функция требует полной авторизации.",
                                                        ];


    public static string GetErrorString(ErrorCode? error) {
        if (error == null)
            return string.Empty;

        return _localization.Language switch {
            Lang.Ru => _ruErrorMessages[(int)error],
            _       => _engErrorMessages[(int)error],
        };
    }

    public static async Task<bool> ReplyWithError(ErrorCode? error, ChatMessage message, ITwitchClient? client) {
        if (client == null || error == null || error == ErrorCode.None) {
            return false;
        }

        await client.SendMessage($"{_localization.GetStr(StrId.ErrorWord)}: {GetErrorString(error)}", message.Id);
        return true;
    }

    public static async Task<bool> SendError(ErrorCode? error, ITwitchClient? client, string? mention = null) {
        if (client == null || error == null || error == ErrorCode.None) {
            return false;
        }

        mention = string.IsNullOrEmpty(mention) ? string.Empty : $"@{mention} ";

        await client.SendMessage($"{mention}Ошибка: {GetErrorString(error)}");
        return true;
    }

    public static void LogMessage(LogLevel logLevel, string message) {
        _logger.Log(logLevel, message);
    }

    public static bool LogError(ErrorCode error) {
        if (error == ErrorCode.None) {
            return false;
        }

        LogMessage(LogLevel.Error, GetErrorString(error));
        return true;
    }

    public static void LogErrorAndPrint(ErrorCode error) {
        var result = LogError(error);
        if (!result) {
            return;
        }

        IoHandler.Clear();
        IoHandler.WriteLine(GetErrorString(error));
        IoHandler.ReadLine();
        IoHandler.Clear();
    }

    public static void LogErrorMessageAndPrint(ErrorCode error, string message) {
        LogError(error);

        IoHandler.Clear();
        IoHandler.WriteLine(message);
        IoHandler.ReadKey();
        IoHandler.Clear();
    }
    
    public static void PrintMessage(LogLevel logLevel, string message) {
        IoHandler.Clear();
        IoHandler.WriteLine(message);
        IoHandler.ReadKey();
        IoHandler.Clear();
    }
}