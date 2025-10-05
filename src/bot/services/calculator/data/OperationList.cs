using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.calculator.data;

public static class OperationList {
    public static Result<double?, ErrorCode?> Addition(double a, double b) {
        return new Result<double?, ErrorCode?>(a + b, null);
    }
    
    public static Result<double?, ErrorCode?> Subtraction(double a, double b) {
        return new Result<double?, ErrorCode?>(a - b, null);
    }
    
    public static Result<double?, ErrorCode?> Multiplication(double a, double b) {
        return new Result<double?, ErrorCode?>(a * b, null);
    }
    
    public static Result<double?, ErrorCode?> Division(double a, double b) {
        if (b == 0) return new Result<double?, ErrorCode?>(null, ErrorCode.Undefined);
        return new Result<double?, ErrorCode?>(a / b, null);
    }
    
    public static Result<double?, ErrorCode?> Modulo(double a, double b) {
        if (b == 0) return new Result<double?, ErrorCode?>(a/100, null);
        return new Result<double?, ErrorCode?>(a % b, null);
    }
    
    public static Result<double?, ErrorCode?> Exponentiation(double a, double b) {
        return new Result<double?, ErrorCode?>(Math.Pow(a, b), null);
    }
    
    public static Result<double?, ErrorCode?> Sin(double a, double b) {
        return new Result<double?, ErrorCode?>(Math.Sin(b*Math.PI/180), null);
    }
    
    public static Result<double?, ErrorCode?> Cos(double a, double b) {
        return new Result<double?, ErrorCode?>(Math.Cos(b*Math.PI/180), null);
    }
    
    public static Result<double?, ErrorCode?> Tan(double a, double b) {
        if (b == 0) return new Result<double?, ErrorCode?>(null, ErrorCode.Undefined);
        return new Result<double?, ErrorCode?>(Math.Tan(b*Math.PI/180), null);
    }
    
    public static Result<double?, ErrorCode?> Log(double a, double b) {
        if (b <= 0) return new Result<double?, ErrorCode?>(null, ErrorCode.Undefined);
        return new Result<double?, ErrorCode?>(Math.Log2(b), null);
    }
    
    public static Result<double?, ErrorCode?> Lg(double a, double b) {
        if (b <= 0) return new Result<double?, ErrorCode?>(null, ErrorCode.Undefined);
        return new Result<double?, ErrorCode?>(Math.Log10(b), null);
    }
}