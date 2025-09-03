﻿using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using TwitchAPI.client;
using TwitchAPI.client.data;

namespace ChatBot.bot.shared.handlers;

public enum ErrorCode {
    InvalidInput,
    TooFewArgs,
    AlreadyContains,
    ServiceDisabled,
    CorruptedCredentials,
    InvalidData,
    NotInitialized,
    ListIsEmpty,
    NotEnoughData,
    SmthWentWrong,
    ClipCreationFailed,
    RequestFailed,
    TranslationFailed,
    ConnectionFailed,
    NoRewardSet,
    UserNotFound,
    None,
}

public static class ErrorHandler {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    private static readonly string[] _internalErrorMessages = [
                                                                  "Invalid Input.",
                                                                  "Too Few Arguments.",
                                                                  "List already contains such element.",
                                                                  "Function is Disabled.",
                                                                  "Corrupted credentials.",
                                                                  "Invalid data.",
                                                                  "Initialize the bot first.",
                                                                  "List is Empty.",
                                                                  "Too Few Data.",
                                                                  "Something Went Wrong.",
                                                                  "Failed to create a clip",
                                                                  "Request Failed.",
                                                                  "Translation failed.",
                                                                  "Connection failed.",
                                                                  "Reward isn't properly set.",
                                                                  "User not found.",
                                                              ];


    private static readonly string[] _twitchErrorMessages = [
                                                                "Неправильный ввод.",
                                                                "Слишком мало аругментов.",
                                                                "Такой элемент уже существует.",
                                                                "Функция отключена.",
                                                                "Некорректные данные для подключения.",
                                                                "Некорректные данные.",
                                                                "Сначала инициализируйте бота.",
                                                                "Пусто.",
                                                                "Слишком мало данных.",
                                                                "Что-то пошло не так.",
                                                                "Не удалось создать клип.",
                                                                "Запрос не удался.",
                                                                "Не удалось перевести.",
                                                                "Не удалось подключиться к сети.",
                                                                "Награда не установлена.",
                                                                "Пользователь не найден.",
                                                            ];
    
    
    public static bool ReplyWithError(ErrorCode error, ChatMessage message, ITwitchClient? client) {
        if (error == ErrorCode.None) {
            return false;
        }
        
        client?.SendReply(message.Id, _twitchErrorMessages[(int)error]);
        return true;
    }

    public static bool LogError(ErrorCode code) {
        if (code == ErrorCode.None) {
            return false;
        }
        
        _logger.Log(LogLevel.Error, _internalErrorMessages[(int)code]);
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
}