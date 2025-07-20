using ChatBot.bot.interfaces;
using ChatBot.Services.Static;
using ChatBot.shared;

namespace ChatBot.Services.presets;

public class Preset {
    public string Name { get; private set; }


    public Preset(string name) {
        Name = name;
    }

    public void Create() {
        Directories.ChangeDataDirectory($"{Name}_data");
        ServiceManager.ServicesToDefault([ServiceName.Presets]);
    }

    public void Load(Bot bot) {
        Directories.ChangeDataDirectory($"{Name}_data");
        ServiceManager.InitServices(bot, [ServiceName.Presets]);
    }
}