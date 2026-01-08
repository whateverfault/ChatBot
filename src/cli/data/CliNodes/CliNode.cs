using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.data.CliNodes;

public enum CliNodePermission {
    Default,
    ReadOnly,
}

public abstract class CliNode {
    protected abstract string Text { get; }

    public virtual bool ShouldSkip => false;


    public virtual int PrintValue(int index, out string end, bool drawIndex = false) {
        end = "\n";

        if (drawIndex) {
            IoHandler.Write($"{index}. ");
        }
        
        IoHandler.Write($"{Text}");
        return 0;
    }
    
    public abstract void Activate(CliState state);
}