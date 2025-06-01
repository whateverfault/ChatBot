namespace ChatBot.CLI;

public class CliState {
    public CliNodeSystem NodeSystem { get; private set; } = null!;
    public CliData Data { get; }


    public CliState(CliData data) {
        Data = data;
    }

    public void Bind(CliNodeSystem system) {
        NodeSystem = system;
    }
}