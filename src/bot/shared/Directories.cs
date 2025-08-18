namespace ChatBot.bot.shared;

public static class Directories {
    private static string _dataDir = "default_data";
    public static string ConfigDirectory => Path.Combine(GetProcessDirectory(), "presets");
    public static string DataDirectory => Path.Combine(ConfigDirectory, _dataDir);
    public static string ServiceDirectory => Path.Combine(DataDirectory, $"services{Path.DirectorySeparatorChar}");


    public static void ChangeDataDirectory(string relativeDirectory) {
        _dataDir = relativeDirectory;
    }

    private static string GetProcessDirectory() {
        int endIndex;
        if (Environment.ProcessPath != null) {
            endIndex = Environment.ProcessPath.LastIndexOf(Path.DirectorySeparatorChar);
        } else {
            return string.Empty;
        }

        return Environment.ProcessPath[..endIndex];
    }
}