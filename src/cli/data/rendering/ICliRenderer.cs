using ChatBot.cli.data.CliNodes.Directories;

namespace ChatBot.cli.data.rendering;

public interface ICliRenderer {
    public void HandleInput(CliState state);
    public void Render(CliState state);
}