namespace ChatBot.cli;

public class CliState {
    public Cli Cli { get; private set; }
    
    public CliRenderer Renderer { get; private set; }
    
    public CliNodeSystem? NodeSystem { get; private set; }
    
    public CliData Data { get; }


    public CliState(Cli cli, CliData data) {
        Cli = cli;
        Data = data;
    }

    public void Bind(CliNodeSystem system, CliRenderer renderer) {
        NodeSystem = system;
        Renderer = renderer;
    }
}