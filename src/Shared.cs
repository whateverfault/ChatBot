namespace ChatBot;

public static class Shared {
    public static readonly string saveDirectory = Path.Combine(Environment.CurrentDirectory, @"data\");
    public const int MaxMessageCapacity = 500;
}