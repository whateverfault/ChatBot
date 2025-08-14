namespace ChatBot.bot.shared.interfaces;

public enum State {
    Disabled,
    Enabled,
}

public abstract class Options {
    protected abstract string Name { get; }
    protected abstract string OptionsPath { get; }

    public abstract State ServiceState { get; }


    public abstract void Save();

    public abstract void Load();

    public abstract void SetDefaults();

    public abstract void SetState(State state);

    public virtual State GetState() {
        return ServiceState;
    }
}