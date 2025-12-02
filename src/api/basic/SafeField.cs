namespace ChatBot.api.basic;

public class SafeField<T> {
    private readonly T _defaultValue;

    private T _valueKeeper;
    public T Value { 
        get => _valueKeeper ?? _defaultValue; 
        set => _valueKeeper = value ?? _defaultValue;
    }
    
    
    public SafeField(T defaultValue) {
        _valueKeeper = defaultValue;
        _defaultValue = defaultValue;
    }
}