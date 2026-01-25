using TwitchAPI.api.data.responses.GetUserInfo;

namespace ChatBot.cli.data.CliNodes.Directories.Users;

public delegate void UserUpdatedHandler();
public delegate bool RemoveUserHandler(int index);

public class CliNodeUsersDynamicDirectory : CliNodeDirectory {
    private readonly CliState _state;
    
    private readonly UserUpdatedHandler _updatedHandler;
    private readonly AddUserHandler _addHandler;
    private readonly RemoveUserHandler _removeHandler;

    protected override string Text { get; }
    
    public override List<CliNode> Nodes { get; }
    public override bool HasBackOption { get; }
    

    public CliNodeUsersDynamicDirectory(
        string text,
        CliState state,
        UserUpdatedHandler updateHandler,
        AddUserHandler addHandler,
        RemoveUserHandler removeHandler,
        Action<EventHandler<UserInfo>> subscribeToUserAdded,
        Action<EventHandler<int>> subscribeToUserRemoved,
        IReadOnlyList<UserInfo> users) {
        Text = text;
        HasBackOption = true;
        
        _state = state;

        _updatedHandler = updateHandler;
        _addHandler = addHandler;
        _removeHandler = removeHandler;
        
        var content = new CliNodeStaticDirectory(
                                                 "Content",
                                                 _state,
                                                 true,
                                                 [
                                                     new CliNodeText(
                                                                     "-----------------------------------",
                                                                     false,
                                                                     true,
                                                                     1
                                                                    ),
                                                 ]
                                                );
        
        foreach (var userInfo in users) {
            content.AddNode(UserToNode(userInfo));
        }

        Nodes = [];
        if (state.NodeSystem != null) {
            Nodes = [
                        new CliNodeAction("Back", state.NodeSystem.DirectoryBack),
                        new CliNodeUserAdd(Add),
                        new CliNodeRemove("Remove User", Remove),
                        content,
                    ];
        }

        subscribeToUserAdded.Invoke((_, userInfo) => { 
                                        content.AddNode(UserToNode(userInfo));
                                    });

        subscribeToUserRemoved((_, index) => { 
                                   content.RemoveNode(index + 2);
                               });
    }

    private void Add(string username) {
        _addHandler.Invoke(username);
    }

    private bool Remove(int index) {
        return _removeHandler.Invoke(index);
    }

    private CliNode UserToNode(UserInfo userInfo) {
        return new CliNodeStaticDirectory(
                                          userInfo.DisplayName,
                                          _state,
                                          true,
                                          [
                                              new CliNodeString(
                                                            "Username",
                                                            userInfo.GetDisplayName,
                                                            CliNodePermission.Default,
                                                            (value) => {
                                                                userInfo.SetDisplayName(value);
                                                                _updatedHandler.Invoke();
                                                            }
                                                            ),
                                          ],
                                          userInfo.GetDisplayName
                                          );
    }
}