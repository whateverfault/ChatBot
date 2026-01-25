using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes.Directories.Users;

public delegate void AddUserHandler(string username);

public class CliNodeUserAdd : CliNode {
    private AddUserHandler _addHandler;
    
    protected override string Text { get; }
    
    
    public CliNodeUserAdd(AddUserHandler addHandler) {
        Text = "Add User";
        
        _addHandler = addHandler;
    }

    public override void Activate(CliState state) {
        var username = IoHandler.ReadLine("Enter Username: ");
        if (string.IsNullOrEmpty(username)) return;
        
        _addHandler.Invoke(username);
    }
}