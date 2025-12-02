namespace ChatBot.bot.interfaces;

public enum State {
    Disabled,
    Enabled,
}

public abstract class Options {
    public abstract State ServiceState { get; }


    public abstract void Save();

    public abstract void Load();

    public abstract void SetDefaults();

    public abstract void SetState(State state);

    public virtual State GetState() {
        return ServiceState;
    }
}