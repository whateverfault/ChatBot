using ChatBot.bot.services.emotes;
using ChatBot.bot.services.emotes.data;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.localization.data;

public enum StrId {
    InvalidStrId,
    GiveawayResult,
    GiveResult,
    LotUsed,
    LotBought,
    LotRemoved,
    LotAdded,
    Balance,
    AllDuelsDeclined,
    DuelDeclined,
    DuelChallenged,
    AcceptDuelResponse0,
    AcceptDuelResponse1,
    AcceptDuelResponse2,
    ChatAdCooldownChanged,
    ChatAdOutputChanged,
    ChatAdNameChanged,
    ChatAdRemoved,
    ChatAdAdded,
    CmdOutputChanged,
    CmdDescriptionChanged,
    CmdRemoved,
    CmdAdded,
    TgNotificationsPromptChanged,
    TgNotificationsServiceStateChanged,
    TgNotificationsServiceStateChangedEveryone,
    EnabledPlural,
    DisabledPlural,
    RewardRemoved,
    RewardCreated,
    GameRequestsReset,
    GameRequestRemoved,
    GameRequestsRewardsReset,
    GameRequestsRewardAdded,
    UseCmdInsideReward,
    PageOutOf,
    Page,
    LangResult,
    DetectLangResult,
    TranslateResult,
    WhoseResult,
    IncorrectWord,
    ErrorWord,
    GuessedMessageCorrect,
    AlreadyGuessed,
    CategoryChanged,
    CurrentCategory,
    UserIsBroadcaster,
    YouAreBroadcaster,
    UserNotFollowed,
    YouNotFollowed,
    UserFollowedFor,
    YouFollowedFor,
    StreamTitleChanged,
    StreamsTitle,
    ClipCreated,
    RequestsRewardSet,
    RequestsStateChanged,
    Mute0,
    Mute1,
    Mute2,
    Mute3,
    Mute4,
    FakeBan,
    BanFromBot,
    FakeUnBan,
    UnBanFromBot,
    When0,
    When1,
    When2,
    When3,
    When4,
    When5,
    Usage,
    BankRewardDescription,
    Cmd0Desc,
    Cmd1Desc,
    Cmd2Desc,
    Cmd3Desc,
    Cmd4Desc,
    Cmd5Desc,
    Cmd6Desc,
    Cmd7Desc,
    Cmd8Desc,
    Cmd9Desc,
    Cmd10Desc,
    Cmd11Desc,
    Cmd12Desc,
    Cmd13Desc,
    Cmd14Desc,
    Cmd15Desc,
    Cmd16Desc,
    Cmd17Desc,
    Cmd18Desc,
    Cmd19Desc,
    Cmd20Desc,
    Cmd21Desc,
    Cmd22Desc,
    Cmd23Desc,
    Cmd24Desc,
    Cmd25Desc,
    Cmd26Desc,
    Cmd27Desc,
    Cmd28Desc,
    Cmd29Desc,
    // 30
    Cmd31Desc,
    Cmd32Desc,
    Cmd33Desc,
    Cmd34Desc,
    Cmd35Desc,
    Cmd36Desc,
    Cmd37Desc,
    Cmd38Desc,
    Cmd39Desc,
    Cmd40Desc,
    Cmd41Desc,
    Cmd42Desc,
    Cmd43Desc,
    Cmd44Desc,
    Cmd45Desc,
    Cmd46Desc,
    Cmd47Desc,
    Cmd48Desc,
    Cmd49Desc,
    Cmd50Desc,
    Cmd51Desc,
    Cmd52Desc,
    Cmd53Desc,
    Cmd54Desc,
    Cmd55Desc,
    Cmd56Desc,
    Cmd57Desc,
    Cmd58Desc,
    Cmd59Desc,
    Cmd60Desc,
    Cmd61Desc,
    Cmd62Desc,
    Cmd63Desc,
    Cmd64Desc,
    Cmd65Desc,
    Cmd66Desc,
    Cmd67Desc,
    Cmd68Desc,
    Cmd69Desc,
    Cmd70Desc,
    Cmd71Desc,
    Cmd72Desc,
    Cmd73Desc,
    Cmd74Desc,
    Cmd75Desc,
    Cmd76Desc,
    Cmd77Desc,
    Cmd78Desc,
    Cmd79Desc,
    Cmd80Desc,
    Cmd81Desc,
    Cmd82Desc,
}

public static class LocalizationsList {
    private static readonly EmotesService _emotes = (EmotesService)Services.Get(ServiceId.Emotes);
    
    private static readonly Dictionary<StrId, Localization> _localizations = new Dictionary<StrId, Localization> {
                                                                                 {
                                                                                     StrId.InvalidStrId,
                                                                                     new Localization(
                                                                                          string.Empty,
                                                                                          string.Empty
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.GiveawayResult,
                                                                                     new Localization(
                                                                                          "Роздано {} {}: {}",
                                                                                          "Given away {} {}: {}"
                                                                                          )
                                                                                 },
                                                                                 {
                                                                                     StrId.GiveResult,
                                                                                     new Localization(
                                                                                          "Успешно отправлено {} {} пользователю {}",
                                                                                          "Successfully sent {} {} to user {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.LotUsed,
                                                                                     new Localization(
                                                                                          "Использовано {}",
                                                                                          "Used {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.LotBought,
                                                                                     new Localization(
                                                                                          "Куплено '{}'. Всего: {}",
                                                                                          "Bought '{}'. Owned: {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.LotRemoved,
                                                                                     new Localization(
                                                                                          "Лот '{}' был удалён",
                                                                                          "Lot '{}' has been removed"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.LotAdded,
                                                                                     new Localization(
                                                                                          "Добавлен новый лот '{}'",
                                                                                          "Lot '{}' has been added"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Balance,
                                                                                     new Localization(
                                                                                          "Баланс: {} {} | За все время: {}{}",
                                                                                          "Balance: {} {} | Earned: {}{}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.AllDuelsDeclined,
                                                                                     new Localization(
                                                                                          "Все ваши дуэли отменены",
                                                                                          "All your duels have been declined"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.DuelDeclined,
                                                                                     new Localization(
                                                                                          "Дуэль отменена",
                                                                                          "Duel has been declined"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.DuelChallenged,
                                                                                     new Localization(
                                                                                          "Вы вызвали {} на дуэль со ставкой {} {}",
                                                                                          "You challenged {} to a duel with bet {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.AcceptDuelResponse0,
                                                                                     new Localization(
                                                                                          "{} выигрывает в дуэли и получает {} {}",
                                                                                          "{} won the duel and got {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.AcceptDuelResponse1,
                                                                                     new Localization(
                                                                                          "{} залутал {} {}",
                                                                                          "{} gained {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.AcceptDuelResponse2,
                                                                                     new Localization(
                                                                                          "{} заовнил {} и выиграл {} {}",
                                                                                          "{} humbled {} and won {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.ChatAdCooldownChanged,
                                                                                     new Localization(
                                                                                          "Перезарядка чат-рекламы '{}' изменена на {}",
                                                                                          "Cooldown of chat-ad '{}' has been changed to {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.ChatAdOutputChanged,
                                                                                     new Localization(
                                                                                          "Вывод чат-рекламы '{}' изменен на '{}'",
                                                                                          "Output of chat-ad '{}' has been changed to '{}'"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.ChatAdNameChanged,
                                                                                     new Localization(
                                                                                          "Название чат-рекламы '{}' изменена на '{}'",
                                                                                          "Name of chat-ad '{}' has been changed to '{}'"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.ChatAdRemoved,
                                                                                     new Localization(
                                                                                          "Чат-реклама '{}' была удалена",
                                                                                          "Chat-ad '{}' has been removed"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.ChatAdAdded,
                                                                                     new Localization(
                                                                                          "Добавлена чат-реклама '{}'",
                                                                                          "Added chat-ad '{}'"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.CmdOutputChanged,
                                                                                     new Localization(
                                                                                          "Вывод команды '{}' изменен на '{}'.",
                                                                                          "Output of command '{}' has been changed to '{}'"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.CmdDescriptionChanged,
                                                                                     new Localization(
                                                                                          "Описание команды '{}' изменено на '{}'.",
                                                                                          "Description of command '{}' has been changed to '{}'"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.CmdRemoved,
                                                                                     new Localization(
                                                                                          "Команда '{}' удалена.",
                                                                                          "Command '{}' has been removed."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.CmdAdded,
                                                                                     new Localization(
                                                                                          "Добавлена новая команда '{}'",
                                                                                          "New command '{}' has been added"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.TgNotificationsPromptChanged,
                                                                                     new Localization(
                                                                                          "Текст уведомлений был изменен",
                                                                                          "Telegram Notifications prompt has been changed"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.TgNotificationsServiceStateChanged,
                                                                                     new Localization(
                                                                                          "Уведомления о стримах теперь {}",
                                                                                          "Telegram Notifications are now {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.TgNotificationsServiceStateChangedEveryone,
                                                                                     new Localization(
                                                                                          "Уведомления о стримах {}",
                                                                                          "Telegram Notifications are {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.EnabledPlural,
                                                                                     new Localization(
                                                                                          "включены",
                                                                                          "enabled"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.DisabledPlural,
                                                                                     new Localization(
                                                                                          "отключены",
                                                                                          "disabled"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.RewardRemoved,
                                                                                     new Localization(
                                                                                          "Награда была удалена",
                                                                                          "Reward has been removed"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.RewardCreated,
                                                                                     new Localization(
                                                                                          "Создана новая награда ({})",
                                                                                          "New reward has been created ({})"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.GameRequestsReset,
                                                                                     new Localization(
                                                                                          "Список заказов очищен",
                                                                                          "Game requests have been reset"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.GameRequestRemoved,
                                                                                     new Localization(
                                                                                          "Игра '{}' удалена из очереди",
                                                                                          "Game '{}' has been removed from queue"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.GameRequestsRewardsReset,
                                                                                     new Localization(
                                                                                          "Список наград для заказа игр был отчищен",
                                                                                          "Game requests rewards list has been reset"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.GameRequestsRewardAdded,
                                                                                     new Localization(
                                                                                          "Награда была добавлена в список",
                                                                                          "Reward has been added"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.UseCmdInsideReward,
                                                                                     new Localization(
                                                                                          "Эта команда должна быть использована внутри награды",
                                                                                          "This command must be used inside a reward"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.PageOutOf,
                                                                                     new Localization(
                                                                                          "Страница {} из {}",
                                                                                          "Page {} out of {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Page,
                                                                                     new Localization(
                                                                                          "Страница {}",
                                                                                          "Page {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.LangResult,
                                                                                     new Localization(
                                                                                          "Язык изменен на '{}'",
                                                                                          "Page {} out of {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.DetectLangResult,
                                                                                     new Localization(
                                                                                          "Самый близкий язык - {}",
                                                                                          "Closest language - {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.TranslateResult,
                                                                                     new Localization(
                                                                                          "Перевод: {}",
                                                                                          "Translation: {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.WhoseResult,
                                                                                     new Localization(
                                                                                          "Это было сообщение от {}",
                                                                                          "It was a message from {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.IncorrectWord,
                                                                                     new Localization(
                                                                                          "Неправильно",
                                                                                          "Incorrect"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.ErrorWord,
                                                                                     new Localization(
                                                                                          "Ошибка",
                                                                                          "Error"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.GuessedMessageCorrect,
                                                                                     new Localization(
                                                                                          "Правильно, это было сообщение от {}",
                                                                                          "Correct, it was a message from {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.AlreadyGuessed,
                                                                                     new Localization(
                                                                                          "Уже отгадано",
                                                                                          "Already guessed"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.CategoryChanged,
                                                                                     new Localization(
                                                                                          "Категория изменена на {}",
                                                                                          "Category changed to {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.CurrentCategory,
                                                                                     new Localization(
                                                                                          "Текущая категория - {}",
                                                                                          "Current category - {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.UserIsBroadcaster,
                                                                                     new Localization(
                                                                                          "{} это владелец канала {}",
                                                                                          "{} is the broadcaster {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.YouAreBroadcaster,
                                                                                     new Localization(
                                                                                          "Вы владелец канала {}",
                                                                                          "You are the broadcaster {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.UserNotFollowed,
                                                                                     new Localization(
                                                                                          "{} не фолловнут на {} {}",
                                                                                          "{} is not following {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.YouNotFollowed,
                                                                                     new Localization(
                                                                                          "Вы не фолловнуты на {} {}",
                                                                                          "You are not following {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.UserFollowedFor,
                                                                                     new Localization(
                                                                                          "{} фолловнут на {} {} {} {}",
                                                                                          "{} has been following {} for {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.YouFollowedFor,
                                                                                     new Localization(
                                                                                          "Вы фолловнуты на {} {} {} {}",
                                                                                          "You have been following {} for {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.StreamTitleChanged,
                                                                                     new Localization(
                                                                                          "Название стрима изменено на {}",
                                                                                          "Stream's title has been changed to {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.StreamsTitle,
                                                                                     new Localization(
                                                                                          "Название стрима - {}",
                                                                                          "Title - {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.ClipCreated,
                                                                                     new Localization(
                                                                                          "Клип создан - {}",
                                                                                          "Clipped - {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.RequestsRewardSet,
                                                                                     new Localization(
                                                                                          "Награда для реквестов была установлена",
                                                                                          "Request's reward has been set"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.RequestsStateChanged,
                                                                                     new Localization(
                                                                                          "Реквесты теперь {}",
                                                                                          "Requests are now {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Mute0,
                                                                                     new Localization(
                                                                                          "{} отошел поспать",
                                                                                          "{} took a nap"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Mute1,
                                                                                     new Localization(
                                                                                          "{} bb",
                                                                                          "{} bb"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Mute2,
                                                                                     new Localization(
                                                                                          "{} решил передохнуть",
                                                                                          "{} has been decided to take a break"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Mute3,
                                                                                     new Localization(
                                                                                          "{} очень устал",
                                                                                          "{} got really tired"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Mute4,
                                                                                     new Localization(
                                                                                          "{} пошел посрать",
                                                                                          "{} went take a dump"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.FakeBan,
                                                                                     new Localization(
                                                                                          "{} отправлен в бан {} {} {}",
                                                                                          "{} got banned {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.BanFromBot,
                                                                                     new Localization(
                                                                                          "{} отстранен от использования бота {} {} {}",
                                                                                          "{} got banned from the bot {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.FakeUnBan,
                                                                                     new Localization(
                                                                                          "{} вышел из бана {} {} {}",
                                                                                          "{} got unbanned {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.UnBanFromBot,
                                                                                     new Localization(
                                                                                          "{} может сново пользоваться ботом {} {} {}",
                                                                                          "{} got unbanned from the bot {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.When0,
                                                                                     new Localization(
                                                                                          "{} уже завтра! PewPewPew PewPewPew PewPewPew",
                                                                                          "{} already tomorrow! PewPewPew PewPewPew PewPewPew"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.When1,
                                                                                     new Localization(
                                                                                          "{} на этой недели! {} {} {}",
                                                                                          "{} this week! {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.When2,
                                                                                     new Localization(
                                                                                          "{} в следующем месяце! {} {} {}",
                                                                                          "{} next month! {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.When3,
                                                                                     new Localization(
                                                                                          "{} через год {} {} {}",
                                                                                          "{} in a year {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.When4,
                                                                                     new Localization(
                                                                                          "{} через 69 лет {} {} {}",
                                                                                          "{} in 69 years {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.When5,
                                                                                     new Localization(
                                                                                          "{} никогда {} {} {}",
                                                                                          "{} never {} {} {}"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Usage,
                                                                                     new Localization(
                                                                                          "{}<комманда> \"аргумент1\" \"аргумент2\" ... | {}cmds для списка комманд",
                                                                                          "{}<cmd> \"arg1\" \"arg2\" ... | {}cmds for list of commands"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.BankRewardDescription,
                                                                                     new Localization(
                                                                                          "Дает {} фантиков",
                                                                                          "Gives {} points"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd0Desc,
                                                                                     new Localization(
                                                                                          "список комманд.",
                                                                                          "list of commands."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd1Desc,
                                                                                     new Localization(
                                                                                          "использование комманд.",
                                                                                          "commands usage."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd2Desc,
                                                                                     new Localization(
                                                                                          "время, которое пользователь отслеживает канал.",
                                                                                          "time user has been following the channel."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd3Desc,
                                                                                     new Localization(
                                                                                          "название стрима.",
                                                                                          "stream's title."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd4Desc,
                                                                                     new Localization(
                                                                                          "изменяет название стрима.",
                                                                                          "changes stream's title."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd5Desc,
                                                                                     new Localization(
                                                                                          "категория стрима.",
                                                                                          "stream's category."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd6Desc,
                                                                                     new Localization(
                                                                                          "изменяет категорию стрима.",
                                                                                          "changes stream's category."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd7Desc,
                                                                                     new Localization(
                                                                                          "создает клип.",
                                                                                          "creates clip."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd8Desc,
                                                                                     new Localization(
                                                                                          "состояние реквестов.",
                                                                                          "requests state."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd9Desc,
                                                                                     new Localization(
                                                                                          "понг!",
                                                                                          "pong!"
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd10Desc,
                                                                                     new Localization(
                                                                                          "{}",
                                                                                          "{}",
                                                                                          () => _emotes.Get7TvEmote(EmoteId.Wait)
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd11Desc,
                                                                                     new Localization(
                                                                                          "{}",
                                                                                          "{}",
                                                                                          () => _emotes.Get7TvEmote(EmoteId.Jail)
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd12Desc,
                                                                                     new Localization(
                                                                                          "спросить ии.",
                                                                                          "ask AI."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd13Desc,
                                                                                     new Localization(
                                                                                          "генерирует сообщение.",
                                                                                          "generates a message."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd14Desc,
                                                                                     new Localization(
                                                                                          "зарандомить сообщение.",
                                                                                          "randomly picks a message."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd15Desc,
                                                                                     new Localization(
                                                                                          "переключает состояние реквестов.",
                                                                                          "switches requests state."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd16Desc,
                                                                                     new Localization(
                                                                                          "установить награду для реквестов.",
                                                                                          "sets request's reward."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd17Desc,
                                                                                     new Localization(
                                                                                          "{}",
                                                                                          "{}",
                                                                                          () => _emotes.Get7TvEmote(EmoteId.Jail)
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd18Desc,
                                                                                     new Localization(
                                                                                          "угадать ник написавшего рандомное сообщение.",
                                                                                          "guess random's message sender username."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd19Desc,
                                                                                     new Localization(
                                                                                          "узнать ник написавшего рандомное сообщение.",
                                                                                          "outputs random's message sender username."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd20Desc,
                                                                                     new Localization(
                                                                                          "повторяет прошлое рандомное сообщение.",
                                                                                          "repeats previous randomly picked message."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd21Desc,
                                                                                     new Localization(
                                                                                          "перевод текста.",
                                                                                          "text translation."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd22Desc,
                                                                                     new Localization(
                                                                                          "узнать язык текста.",
                                                                                          "detect text's language."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd23Desc,
                                                                                     new Localization(
                                                                                          "установить язык переводчика по умолчанию.",
                                                                                          "sets default translator's language."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd25Desc,
                                                                                     new Localization(
                                                                                          "уровень стоящий на данной позиции по AREDL.",
                                                                                          "level placed on the given spot according to AREDL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd26Desc,
                                                                                     new Localization(
                                                                                          "позиция уровня по AREDL.",
                                                                                          "level's position according to AREDL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd27Desc,
                                                                                     new Localization(
                                                                                          "случайный экстрим демон.",
                                                                                          "randomly picks an extreme demon."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd28Desc,
                                                                                     new Localization(
                                                                                          "сложнейший экстрим по AREDL.",
                                                                                          "hardest extreme according to AREDL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd29Desc,
                                                                                     new Localization(
                                                                                          "легчайший экстрим по AREDL.",
                                                                                          "easiest extreme according to AREDL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd31Desc,
                                                                                     new Localization(
                                                                                          "уровень стоящий на данной позиции по AREPL.",
                                                                                          "level placed on the given spot according to AREPL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd32Desc,
                                                                                     new Localization(
                                                                                          "позиция уровня по AREPL.",
                                                                                          "level's position according to AREPL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd33Desc,
                                                                                     new Localization(
                                                                                          "Информация о клане по AREDL.",
                                                                                          "clan's info according to AREDL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd34Desc,
                                                                                     new Localization(
                                                                                          "хардест клана по AREDL.",
                                                                                          "clan's hardest according to AREDL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd35Desc,
                                                                                     new Localization(
                                                                                          "случайный пройденный кланом экстрим.",
                                                                                          "randomly picked clan's completion."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd36Desc,
                                                                                     new Localization(
                                                                                          "список заказов игр.",
                                                                                          "game requests queue."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd37Desc,
                                                                                     new Localization(
                                                                                          "добавляет игру в очередь.",
                                                                                          "adds game into queue."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd38Desc,
                                                                                     new Localization(
                                                                                          "удаляет игру из очередь.",
                                                                                          "removes game from queue."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd39Desc,
                                                                                     new Localization(
                                                                                          "очищает очередь заказов игр.",
                                                                                          "wipes the game requests queue."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd40Desc,
                                                                                     new Localization(
                                                                                          "добавить награду для заказа игр.",
                                                                                          "adds a game's requests reward."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd41Desc,
                                                                                     new Localization(
                                                                                          "очистить список наград для заказа игр.",
                                                                                          "wipes game's requests rewards list."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd42Desc,
                                                                                     new Localization(
                                                                                          "создать награду.",
                                                                                          "creates reward."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd43Desc,
                                                                                     new Localization(
                                                                                          "удалить награду.",
                                                                                          "removes reward."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd44Desc,
                                                                                     new Localization(
                                                                                          "состояние уведомлений.",
                                                                                          "telegram notifications state."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd45Desc,
                                                                                     new Localization(
                                                                                          "перключить состояние уведомлений.",
                                                                                          "switch telegram notifications state."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd46Desc,
                                                                                     new Localization(
                                                                                          "изменить текст уведомления.",
                                                                                          "changes telegram notifications prompt."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd47Desc,
                                                                                     new Localization(
                                                                                          "список дополнительных комманд.",
                                                                                          "list of custom commands."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd48Desc,
                                                                                     new Localization(
                                                                                          "список команд.",
                                                                                          "list of commands."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd49Desc,
                                                                                     new Localization(
                                                                                          "добавить команду.",
                                                                                          "adds command."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd50Desc,
                                                                                     new Localization(
                                                                                          "удалить команду.",
                                                                                          "removes command."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd51Desc,
                                                                                     new Localization(
                                                                                          "изменить описание команды.",
                                                                                          "changes command's description."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd52Desc,
                                                                                     new Localization(
                                                                                          "изменить вывод команды.",
                                                                                          "changes command's output."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd53Desc,
                                                                                     new Localization(
                                                                                          "список чат-реклам.",
                                                                                          "list of chat-ads."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd54Desc,
                                                                                     new Localization(
                                                                                          "добавить чат-рекламу.",
                                                                                          "adds chat-ad."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd55Desc,
                                                                                     new Localization(
                                                                                          "удалить чат-рекламу.",
                                                                                          "removes chat-ad."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd56Desc,
                                                                                     new Localization(
                                                                                          "изменить название чат-рекламы.",
                                                                                          "changes chat-ad's name."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd57Desc,
                                                                                     new Localization(
                                                                                          "изменить описание чат-рекламы.",
                                                                                          "changes chat-ad's description."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd58Desc,
                                                                                     new Localization(
                                                                                          "изменить перезарядку чат-рекламы.",
                                                                                          "changes chat-ad's cooldown."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd59Desc,
                                                                                     new Localization(
                                                                                          "сложнейший платформер по AREPL.",
                                                                                          "hardest platformer according to AREPL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd60Desc,
                                                                                     new Localization(
                                                                                          "легчайший платформер по AREPL.",
                                                                                          "easiest platformer according to AREPL."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd61Desc,
                                                                                     new Localization(
                                                                                          "вывести баланс фантиков.",
                                                                                          "outputs your points balance."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd62Desc,
                                                                                     new Localization(
                                                                                          "список лотов.",
                                                                                          "list of lots."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd63Desc,
                                                                                     new Localization(
                                                                                          "слить фантики.",
                                                                                          "waste points."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd64Desc,
                                                                                     new Localization(
                                                                                          "подать милостыню фантиками.",
                                                                                          "give alms in points."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd65Desc,
                                                                                     new Localization(
                                                                                          "раздать фантики.",
                                                                                          "giveaways your points."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd66Desc,
                                                                                     new Localization(
                                                                                          "список наград для покупки фантиков.",
                                                                                          "list of rewards to buy points."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd67Desc,
                                                                                     new Localization(
                                                                                          "создать награду для покупки фантиков.",
                                                                                          "creates a reward to buy points."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd68Desc,
                                                                                     new Localization(
                                                                                          "удалить награду для покупки фантиков.",
                                                                                          "removes a reward to buy points."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd69Desc,
                                                                                     new Localization(
                                                                                          "выполнить выражение.",
                                                                                          "executes expression."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd70Desc,
                                                                                     new Localization(
                                                                                          "предложить дуэль на фантики.",
                                                                                          "duel for points proposal."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd71Desc,
                                                                                     new Localization(
                                                                                          "принять дэуль на фантики.",
                                                                                          "accept duel proposal."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd72Desc,
                                                                                     new Localization(
                                                                                          "отклонить дэуль на фантики.",
                                                                                          "reject duel proposal."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd73Desc,
                                                                                     new Localization(
                                                                                          "отклонить все дэули на фантики.",
                                                                                          "reject all duel proposals."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd74Desc,
                                                                                     new Localization(
                                                                                          "список предложенных дуэлей",
                                                                                          "list of duel proposals."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd75Desc,
                                                                                     new Localization(
                                                                                          "купить лот.",
                                                                                          "use to buy lot."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd24Desc,
                                                                                     new Localization(
                                                                                          "использовать купленный лот.",
                                                                                          "uses bought lot."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd77Desc,
                                                                                     new Localization(
                                                                                          "добавить лот.",
                                                                                          "adds lot."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd78Desc,
                                                                                     new Localization(
                                                                                          "удалить лот.",
                                                                                          "removes lot."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd76Desc,
                                                                                     new Localization(
                                                                                          "профиль пользователя по AREDL.",
                                                                                          "user's AREDL profile."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd79Desc,
                                                                                     new Localization(
                                                                                          "мутит пользователя за фантики.",
                                                                                          "mutes user for points."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd80Desc,
                                                                                     new Localization(
                                                                                          "случайный экстрим платформер.",
                                                                                          "randomly picks an extreme platformer."
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd81Desc,
                                                                                     new Localization(
                                                                                          "{}",
                                                                                          "{}",
                                                                                          () => _emotes.Get7TvEmote(EmoteId.Happy)
                                                                                         )
                                                                                 },
                                                                                 {
                                                                                     StrId.Cmd82Desc,
                                                                                     new Localization(
                                                                                          "{}",
                                                                                          "{}",
                                                                                          () => _emotes.Get7TvEmote(EmoteId.Happy)
                                                                                         )
                                                                                 },
                                                                             };


    public static Localization? GetLocalization(StrId id) {
        return _localizations.GetValueOrDefault(id);
    }
}