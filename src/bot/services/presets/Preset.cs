using ChatBot.bot.services.Static;
using ChatBot.shared;

namespace ChatBot.bot.services.presets;

public class Preset {
    private bool _firstLoad = true;
    
    public string Name { get; private set; }


    public Preset(string name) {
        Name = name;
    }

    public void Create() {
        Directories.ChangeDataDirectory($"{Name}_data");
        ServiceManager.ServicesToDefault([ServiceName.Presets,]);

        var bot = TwitchChatBot.Instance;
        bot.Options.SetDefaults();
        bot.Stop();
    }

    public void Load() {
        Directories.ChangeDataDirectory($"{Name}_data");
        
        ServiceManager.LoadServices([ServiceName.Presets,]);
        
        var bot = TwitchChatBot.Instance;
        bot.Options.Load();

        if (Program.AutoInit && _firstLoad) {
            _firstLoad = false;
            return;
        }
        bot.Stop();
    }
}