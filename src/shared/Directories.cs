namespace ChatBot.shared;

public static class Directories {
    private static string _dataDir = @"default_data\";
    public static string ConfigDirectory => Path.Combine(GetProcessDirectory(), @"presets\");
    public static string DataDirectory => Path.Combine(ConfigDirectory, _dataDir);
    public static string ServiceDirectory => Path.Combine(DataDirectory, @"services\");


    public static void ChangeDataDirectory(string relativeDirectory) {
        _dataDir = relativeDirectory;
    }

    private static string GetProcessDirectory() {
        int endIndex;
        if (Environment.ProcessPath != null) {
            endIndex = Environment.ProcessPath.LastIndexOf('\\');
            if (endIndex == -1) {
                endIndex = Environment.ProcessPath.LastIndexOf('/');
            }
        } else {
            return string.Empty;
        }

        return Environment.ProcessPath[0..endIndex];
    }
}