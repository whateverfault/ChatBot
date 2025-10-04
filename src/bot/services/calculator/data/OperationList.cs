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
        if (b == 0) return new Result<double?, ErrorCode?>(a / b, ErrorCode.DivisionByZero);
        return new Result<double?, ErrorCode?>(a / b, null);
    }
    
    public static Result<double?, ErrorCode?> Modulo(double a, double b) {
        if (b == 0) return new Result<double?, ErrorCode?>(a / b, ErrorCode.DivisionByZero);
        return new Result<double?, ErrorCode?>(a % b, null);
    }
    
    public static Result<double?, ErrorCode?> Exponentiation(double a, double b) {
        return new Result<double?, ErrorCode?>(Math.Pow(a, b), null);
    }
}