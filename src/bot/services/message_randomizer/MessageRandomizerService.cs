using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_logs;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.client.data;

namespace ChatBot.bot.services.message_randomizer;

public class MessageRandomizerService : Service {
    private static ITwitchClient? Client => TwitchChatBot.Instance.GetClient();
    private static ChatLogsService ChatLogs => (ChatLogsService)ServiceManager.GetService(ServiceName.ChatLogs);

    public override string Name => ServiceName.MessageRandomizer;
    public override MessageRandomizerOptions Options { get; } = new MessageRandomizerOptions();


    public void HandleChatLog(ChatMessage message) {
        if (Options.ServiceState == State.Disabled) return;
        HandleCounter();
    }

    public void HandleCounter() {
        if (Options.ServiceState == State.Disabled) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.ServiceDisabled);
            return;
        }
        if (Options.Randomness == State.Enabled && Options.Counter < Options.RandomValue
            || Options.Randomness == State.Disabled && Options.Counter < Options.CounterMax) {
            Options.IncreaseCounter();
            return;
        }

        if (Options.Randomness == State.Enabled) {
            Options.SetRandomValue();
        }

        var err = Generate(out var message);
        if (ErrorHandler.LogError(err)) {
            return;
        }
        Client?.SendMessage(message!.Text);
    }

    public ErrorCode Generate(out Message? message) {
        message = null;
        
        var logs = ChatLogs.GetLogs();
        if (logs.Count <= 0) {
            return ErrorCode.NotEnoughData;
        }

        var randomIndex = Random.Shared.Next(0, logs.Count);
        Options.SetLastGeneratedMessage(logs[randomIndex]);
        Options.SetMessageState(MessageState.NotGuessed);
        
        Options.ZeroCounter();
        Options.IncreaseCounter();
        
        message = Options.LastGeneratedMessage;
        return ErrorCode.None;
    }

    public void GenerateAndSend() {
        var err = Generate(out var message);
        if (ErrorHandler.LogError(err)
            || message == null) return;
        
        Client?.SendMessage(message.Text);
    }

    public ErrorCode GetLastGeneratedMessage(out Message? message) {
        message = Options.LastGeneratedMessage;
        if (Options.ServiceState == State.Disabled) {
            return ErrorCode.ServiceDisabled;
        }
        return string.IsNullOrEmpty(Options.LastGeneratedMessage.Text)? ErrorCode.NotEnoughData : ErrorCode.None;
    }
    
    public override State GetServiceState() {
        return Options.ServiceState;
    }
    
    public int GetRandomnessAsInt() {
        return (int)Options.Randomness;
    }
    
    public void RandomnessNext() {
        Options.SetRandomnessState((State)(((int)Options.Randomness+1)%Enum.GetValues(typeof(State)).Length));
    }

    public int GetCounter() {
        return Options.Counter;
    }

    public override void Init() {
        base.Init();
        
        Options.SetRandomValue();
    }
}