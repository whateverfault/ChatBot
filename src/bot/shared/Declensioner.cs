namespace ChatBot.bot.shared;

public static class Declensioner {
    public static string Years(long years) {
        var lastTwoDigits = years%100;
        var lastDigit = years%10;
        
        return lastDigit switch {
                   1 when lastTwoDigits != 11                                     => "год",
                   > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "года",
                   _                                                              => "лет",
               };
    }

    public static string Months(int months) {
        var lastTwoDigits = months%100;
        var lastDigit = months%10;
        
        return lastDigit switch {
                   1 when lastTwoDigits != 11                                     => "месяц",
                   > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "месяца",
                   _                                                              => "месяцев",
               };
    }
    
    public static string Days(int days) {
        var lastTwoDigits = days%100;
        var lastDigit = days%10;
        
        return lastDigit switch {
                   1 when lastTwoDigits != 11                                     => "день",
                   > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "дня",
                   _                                                              => "дней",
               };
    }

    public static string Hours(int hours) {
        var lastTwoDigits = hours%100;
        var lastDigit = hours%10;
        
        return lastDigit switch {
                   1 when lastTwoDigits != 11                                     => "час",
                   > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "часа",
                   _                                                              => "часов",
               };
    }
    
    public static string Mins(int mins) {
        var lastTwoDigits = mins%100;
        var lastDigit = mins%10;
        
        return lastDigit switch {
                   1 when lastTwoDigits != 11                                     => "минуту",
                   > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "минуты",
                   _                                                              => "минут",
               };
    }
    
    public static string Secs(int secs) {
        var lastTwoDigits = secs%100;
        var lastDigit = secs%10;
        
        return lastDigit switch {
                   1 when lastTwoDigits != 11                                     => "секунду",
                   > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "секунды",
                   _                                                              => "секунд",
               };
    }

    public static string Points(long amount) {
        var lastTwoDigits = amount%100;
        var lastDigit = amount%10;
        
        return lastDigit switch {
                   1 when lastTwoDigits != 11                                     => "фантик",
                   > 1 and < 5 when lastTwoDigits is not 12 and not 13 and not 14 => "фантика",
                   _                                                              => "фантиков",
               };
    }
}