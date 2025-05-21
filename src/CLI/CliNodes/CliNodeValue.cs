namespace ChatBot.CLI.CliNodes;

public class CliNodeValue<T> : CliNode {
    public delegate T Getter();
    public delegate void Setter(T value);
        
    public override string Text { get; }
    public override CliNodeType Type { get; }
    public override Type ValueType { get; }
    public Getter ValueGetter { get; }
    public Setter ValueSetter { get; }
    public bool IsReadOnly { get; }


    public CliNodeValue(string text, Getter valueGetter, Setter valueSetter, Type valueType, bool isReadOnly = false) {
        Type = CliNodeType.Value;

        Text = text;
        ValueGetter = valueGetter;
        ValueSetter = valueSetter;
        ValueType = valueType;
        IsReadOnly = isReadOnly;
    }
}