using ChatBot.Services.message_randomizer;
using ChatBot.utils;

namespace ChatBot.Shared.interfaces;

public enum State {
    Disabled,
    Enabled,
}

public abstract class Options {
    protected abstract string Name { get; }
    protected abstract string OptionsPath { get; }
    
    public abstract State ServiceState { get; }


    public abstract void Save();

    public abstract bool Load();
}