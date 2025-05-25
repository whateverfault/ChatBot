using TwitchLib.Client.Interfaces;

namespace ChatBot.CLI.CliNodes;

public enum CliNodeType {
    Directory,
    Bool,
    Value,
    Action,
    WriteOnly,
    Counter,
    State,
    Client,
    ActionWithGetter,
    DynamicDirectory,
    NodeAdder,
    NodeRemover,
    Text
}

public enum CliNodeValueType {
    String,
    Range,
    Int,
    Char,
    State
}

public enum CliNodePermission {
    Default,
    ReadOnly,
    WriteOnly,
}

public delegate void ActionHandler();
public delegate dynamic Getter();
public delegate void Setter(dynamic value);
public delegate void NodeRemover(int index);
public delegate void NodeAdder(CliNode node);
public delegate void ClientHandler(ITwitchClient client, string channel);

public class CliNode {
    private List<CliNode> _nodes;
    private bool _hasBackOption;
    
    public string Text { get; }
    public CliNodeType Type { get; }

    public ActionHandler Action { get; }
    
    public CliNodeValueType ValueType { get; }
    public Getter ValueGetter { get; }
    public Setter ValueSetter { get; }
    public CliNodePermission Permission { get; }

    public NodeAdder NodeAppend { get; }
    public NodeRemover NodeRemove { get; }
    
    public ClientHandler ClientAction { get; }
    
    public List<CliNode> Nodes => _nodes;
    public List<CliNode> OuterNodes { get; } = [];

    public bool HasBackOption => _hasBackOption;

    
    public CliNode(string text, ActionHandler back) {
        Text = text;
        Action = back;
        
        Type = CliNodeType.Action;
        Permission = CliNodePermission.ReadOnly;
    }
    
    public CliNode(string text) {
        Text = text;
        
        Type = CliNodeType.Text;
        Permission = CliNodePermission.ReadOnly;
    }
    
    public CliNode(string text, Getter getter, ActionHandler action, CliNodeValueType valueType) {
        Text = text;
        ValueType = valueType;
        ValueGetter = getter;
        Action = action;
        
        Type = CliNodeType.ActionWithGetter;
        Permission = CliNodePermission.Default;
    }
    
    public CliNode(string text, Getter getter, CliNodeValueType valueType) {
        Text = text;
        ValueType = valueType;
        ValueGetter = getter;
        
        Type = CliNodeType.Value;
        Permission = CliNodePermission.ReadOnly;
    }
    
    public CliNode(string text, Getter getter, Setter setter, CliNodeValueType valueType) {
        Text = text;
        ValueType = valueType;
        ValueGetter = getter;
        ValueSetter = setter;
        
        Type = CliNodeType.Value;
        Permission = CliNodePermission.Default;
    }
    
    public CliNode(string text, Setter setter, CliNodeValueType valueType) {
        Text = text;
        ValueType = valueType;
        ValueSetter = setter;
        
        Type = CliNodeType.Value;
        Permission = CliNodePermission.WriteOnly;
    }
    
    public CliNode(string text, string addText, string removeText, ActionHandler back, CliNodeValueType valueType) {
        Text = text;
        ValueType = valueType;
        
        OuterNodes = [
                          new CliNode(addText, AddNode),
                          new CliNode(removeText, RemoveNode),
                      ];
        _nodes = [
                     new CliNode("Back", back)
                 ];
        _hasBackOption = true;
        
        Type = CliNodeType.DynamicDirectory;
        Permission = CliNodePermission.ReadOnly;
    }
    
    public CliNode(string text,  Getter getter, ClientHandler client, CliNodeValueType valueCliNodeValueType) {
        Text = text;
        ValueType = valueCliNodeValueType;
        ValueGetter = getter;
        ClientAction = client;
        
        Type = CliNodeType.Client;
        Permission = CliNodePermission.ReadOnly;
    }
    
    public CliNode(string text, ClientHandler client) {
        Text = text;
        ClientAction = client;
        
        Type = CliNodeType.Client;
        Permission = CliNodePermission.WriteOnly;
    }
    
    public CliNode(string text, ActionHandler? back, params CliNode[][] nodes) {
        Text = text;
        
        _nodes = [];
        if (back != null) {
            _nodes.Add(new CliNode("Back", back));
            _hasBackOption = true;
        }
        if (nodes.Length > 0) {
            foreach (var subnodes in nodes) {
                _nodes.AddRange(subnodes);
            }
        }
        
        Type = CliNodeType.Directory;
        Permission = CliNodePermission.ReadOnly;
    }

    private CliNode(string text, NodeAdder add) {
        Text = text;
        NodeAppend = add;
        
        Type = CliNodeType.NodeAdder;
        Permission = CliNodePermission.WriteOnly;
    }
    
    private CliNode(string text, NodeRemover remove) {
        Text = text;
        NodeRemove = remove;
        
        Type = CliNodeType.NodeRemover;
        Permission = CliNodePermission.WriteOnly;
    }
    
    public void AddBackOption(ActionHandler back) {
        if (HasBackOption) return;
        _hasBackOption = true;

        var temp = Nodes;

        _nodes = [
                     new CliNode("Back", back)
                 ];

        Nodes.AddRange(temp);
    }

    private void AddNode(CliNode node) {
        if (Type != CliNodeType.DynamicDirectory) return;
        
        Nodes.Add(node);
    }

    private void RemoveNode(int index) {
        if (Type != CliNodeType.DynamicDirectory) return;
        
        if (index < 1 || index >= Nodes.Count) return;
        
        Nodes.RemoveAt(index);
    }
}