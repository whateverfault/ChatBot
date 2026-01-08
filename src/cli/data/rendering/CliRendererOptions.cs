using ChatBot.api.json;
using ChatBot.cli.data.rendering.saved;

namespace ChatBot.cli.data.rendering;

public class CliRendererOptions {
    private const string NAME = "cli";
    
    private readonly object _fileLock = new object();
    
    private static string ConfigDirectory => Path.Combine(GetProcessDirectory(), "presets");
    private static string OptionsPath => Path.Combine(Path.Combine(ConfigDirectory, NAME), $"{NAME}.json");

    private SaveData? _saveData;

    public Renderer CurrentRenderer => _saveData!.CurrentRenderer;
    
    
    public void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    private void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Path.Combine(ConfigDirectory, NAME), _saveData);
        }
    }

    public void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }

    public void SetRenderer(Renderer value) {
        _saveData!.CurrentRenderer = value;
        Save();
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