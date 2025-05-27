namespace ChatBot.shared;

public static class Directories {
    public static readonly string dataDirectory = Path.Combine(Environment.CurrentDirectory, @"data\");
    public static readonly string serviceDirectory = Path.Combine(dataDirectory, @"services\");
}