using TwitchLib.Client.Interfaces;

namespace ChatBot.CLI.CliNodes;

public delegate void ActionHandler();

public class CliNodeGeneric<T> : CliNode {
    private List<CliNode> _nodes;
    private bool _hasBackOption;

    public delegate T Getter();
    public delegate void Setter(T value);
    public delegate void ClientHandler(ITwitchClient client, string channel);
    
    public override string Text { get; }
    public override CliNodeType Type { get; }

    public override ActionHandler Action { get; }
    
    public override CliNodeValueType ValueType { get; }
    public Getter ValueGetter { get; }
    public Setter ValueSetter { get; }
    public override CliNodePermission Permission { get; }

    public ClientHandler ClientAction { get; }
    
    public override List<CliNode> Nodes => _nodes;
    public override bool HasBackOption => _hasBackOption;

    
    public CliNodeGeneric(string text, ActionHandler action) {
        Text = text;
        Action = action;
        
        Type = CliNodeType.Action;
    }
    
    public CliNodeGeneric(string text, Getter getter, ActionHandler action, CliNodeValueType valueCliNodeValueType) {
        Text = text;
        ValueType = valueCliNodeValueType;
        ValueGetter = getter;
        Action = action;
        
        Type = CliNodeType.ActionWithGetter;
        Permission = CliNodePermission.Default;
    }
    
    public CliNodeGeneric(string text, Getter getter, CliNodeValueType valueCliNodeValueType) {
        Text = text;
        ValueType = valueCliNodeValueType;
        ValueGetter = getter;
        
        Type = CliNodeType.Value;
        Permission = CliNodePermission.ReadOnly;
    }
    
    public CliNodeGeneric(string text, Getter getter, Setter setter, CliNodeValueType valueCliNodeValueType) {
        Text = text;
        ValueType = valueCliNodeValueType;
        ValueGetter = getter;
        ValueSetter = setter;
        
        Type = CliNodeType.Value;
        Permission = CliNodePermission.Default;
    }
    
    public CliNodeGeneric(string text, Setter setter, CliNodeValueType valueCliNodeValueType) {
        Text = text;
        ValueType = valueCliNodeValueType;
        ValueSetter = setter;
        
        Type = CliNodeType.Value;
        Permission = CliNodePermission.WriteOnly;
    }
    
    public CliNodeGeneric(string text,  Getter getter, ClientHandler client, CliNodeValueType valueCliNodeValueType) {
        Text = text;
        ValueType = valueCliNodeValueType;
        ValueGetter = getter;
        ClientAction = client;
        
        Type = CliNodeType.Client;
        Permission = CliNodePermission.ReadOnly;
    }
    
    public CliNodeGeneric(string text, ClientHandler client) {
        Text = text;
        ClientAction = client;
        
        Type = CliNodeType.Client;
        Permission = CliNodePermission.WriteOnly;
    }
    
    public CliNodeGeneric(string text, CliNode[] nodes, ActionHandler? back = null) {
        Text = text;
        
        _nodes = [];
        if (back != null) {
            _nodes.Add(new CliNodeGeneric<T>("Back", back));
            _hasBackOption = true;
        }
        if (nodes.Length > 0) {
            _nodes.AddRange(nodes);
        }
        
        Type = CliNodeType.Directory;
    }

    public void AddBackOption(ActionHandler back) {
        if (HasBackOption) return;
        _hasBackOption = true;

        var temp = Nodes;

        _nodes = [
                     new CliNodeGeneric<T>("Back", back)
                 ];

        Nodes.AddRange(temp);
    }
}