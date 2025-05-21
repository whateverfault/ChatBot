namespace ChatBot.shared.interfaces;

public enum State {
    Disabled,
    Enabled,
}

public abstract class Options {
    protected abstract string Name { get; }
    protected abstract string OptionsPath { get; }
    
    public abstract State State { get; }


    public abstract void Save();

    public abstract bool TryLoad();
    
    public abstract void Load();

    public abstract void SetDefaults();

    public abstract void SetState(State state);

    public abstract State GetState();
}