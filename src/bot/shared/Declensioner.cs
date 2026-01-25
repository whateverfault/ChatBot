using ChatBot.bot.services.localization;
using ChatBot.bot.services.localization.data;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.shared;

public static class Declensioner {
    private static readonly LocalizationService _localization = (LocalizationService)Services.Get(ServiceId.Localization);
    
    
    public static string Years(long years) {
        var lastTwoDigits = years%100;
        var lastDigit = years%10;

        return _localization.Language switch {
            Lang.Ru => lastDigit switch {
                1 when lastTwoDigits != 11                                     => "год",
                > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "года",
                _                                                              => "лет",
            },
            Lang.Eng => lastDigit switch {
                1 => "year",
                _ => "years",
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static string Months(int months) {
        var lastTwoDigits = months%100;
        var lastDigit = months%10;

        return _localization.Language switch {
            Lang.Ru => lastDigit switch {
                1 when lastTwoDigits != 11                                     => "месяц",
                > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "месяца",
                _                                                              => "месяцев",
            },
            Lang.Eng => lastDigit switch {
                1 => "month",
                _ => "months",
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
    
    public static string Days(int days) {
        var lastTwoDigits = days%100;
        var lastDigit = days%10;

        return _localization.Language switch {
            Lang.Ru => lastDigit switch {
                1 when lastTwoDigits != 11                                     => "день",
                > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "дня",
                _                                                              => "дней",
            },
            Lang.Eng => lastDigit switch {
                1 => "day",
                _ => "days",
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static string Hours(int hours) {
        var lastTwoDigits = hours%100;
        var lastDigit = hours%10;

        return _localization.Language switch {
            Lang.Ru => lastDigit switch {
                1 when lastTwoDigits != 11                                     => "час",
                > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "часа",
                _                                                              => "часов",
            },
            Lang.Eng => lastDigit switch {
                1 => "hour",
                _ => "hours",
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
    
    public static string Mins(int mins) {
        var lastTwoDigits = mins%100;
        var lastDigit = mins%10;

        return _localization.Language switch {
            Lang.Ru => lastDigit switch {
                1 when lastTwoDigits != 11                                     => "минуту",
                > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "минуты",
                _                                                              => "минут",
            },
            Lang.Eng => lastDigit switch {
                1 => "minute",
                _ => "minutes",
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
    
    public static string Secs(long secs) {
        var lastTwoDigits = secs%100;
        var lastDigit = secs%10;

        return _localization.Language switch {
            Lang.Ru => lastDigit switch {
                1 when lastTwoDigits != 11                                     => "секунду",
                > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "секунды",
                _                                                              => "секунд",
            },
            Lang.Eng => lastDigit switch {
                1 => "second",
                _ => "seconds",
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static string Points(long amount) {
        var lastTwoDigits = amount%100;
        var lastDigit = amount%10;

        return _localization.Language switch {
            Lang.Ru => lastDigit switch {
                1 when lastTwoDigits != 11                                     => "фантик",
                > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "фантика",
                _                                                              => "фантиков",
            },
            Lang.Eng => lastDigit switch {
                1 => "point",
                _ => "points",
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
    
    public static string Accounts(long amount) {
        var lastTwoDigits = amount%100;
        var lastDigit = amount%10;

        return _localization.Language switch {
            Lang.Ru => lastDigit switch {
                1 when lastTwoDigits != 11                                     => "аккаунт",
                > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "аккаунта",
                _                                                              => "аккаунтов",
            },
            Lang.Eng => lastDigit switch {
                1 => "account",
                _ => "accounts",
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}