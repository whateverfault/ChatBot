using System.Text.RegularExpressions;

namespace ChatBot.Services.message_filter;

public class CommentedRegex {
    public Regex Regex { get; }
    public bool HasComment { get; }
    public string Comment { get; }


    public CommentedRegex(Regex regex, bool hasComment, string comment) {
        Regex = regex;
        HasComment = hasComment;
        Comment = comment;
    }
}