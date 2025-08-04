using ChatBot.bot;
using ChatBot.services.Static;
using ChatBot.shared;

namespace ChatBot.services.presets;

public class Preset {
    public string Name { get; private set; }


    public Preset(string name) {
        Name = name;
    }

    public void Create() {
        Directories.ChangeDataDirectory($"{Name}_data");
        ServiceManager.ServicesToDefault([ServiceName.Presets,]);
    }

    public void Load() {
        Directories.ChangeDataDirectory($"{Name}_data");
        
        ServiceManager.InitServices([ServiceName.Presets,]);
        
        var bot = TwitchChatBot.Instance;
        bot.Options.Load();
        bot.Stop();
    }
}