namespace ChatBot.CLI.CliNodes.Directories;

public class Content {
    public string ContentString { get; }
    public bool HasComment { get; }
    public string Comment { get; }


    public Content(string content, bool hasComment, string comment) {
        ContentString = content;
        HasComment = hasComment;
        Comment = comment;
    }
}