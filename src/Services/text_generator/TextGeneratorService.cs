using System.Text;
using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Interfaces;

namespace ChatBot.Services.text_generator;

public class TextGeneratorService : Service {
    private readonly Random _random = new();
    private bot.ChatBot _bot = null!;
    private ITwitchClient? Client => _bot.GetClient();

    public override string Name => ServiceName.TextGenerator;
    public override TextGeneratorOptions Options { get; } = new();


    public void Train() {
        var chatLogs = Options.ChatLogsService.GetLogs();
        var sb = new StringBuilder();
        
        foreach (var message in chatLogs) {
            sb.AppendLine(message.Msg);
        }
        
        Options.Train(sb.ToString());
    }
    
    public ErrorCode Generate(out string message) {
        message = string.Empty;
        if (Options.Model.Count == 0) return ErrorCode.InvalidData;

        var context = Options.Model.Keys.ElementAt(_random.Next(Options.Model.Keys.Count));
        var result = new List<string>(context.Split(' '));

        for (var i = 0; i < Options.MaxLength; i++) {
            if (!Options.Model.TryGetValue(context, out var options) || options.Count == 0)
                break;

            var nextWord = WeightedRandomChoice(options);
            result.Add(nextWord);

            var contextWords = context.Split(' ').Skip(1).Append(nextWord).ToArray();
            context = string.Join(" ", contextWords);
        }

        message = string.Join(" ", result);
        return ErrorCode.None;
    }

    public void GenerateAndSend() {
        if (Options.ServiceState == State.Disabled) {
            ErrorHandler.LogError(ErrorCode.ServiceDisabled);
            return;
        }

        var err = Generate(out var message);
        if (ErrorHandler.LogError(err)) {
            return;
        }
        
        Client?.SendMessage(_bot.Options.Channel!, message);
    }
    
    private string WeightedRandomChoice(Dictionary<string, int> modelOptions)
    {
        var total = modelOptions.Sum(x => x.Value);
        var randomValue = _random.Next(total);

        foreach (var option in modelOptions)
        {
            if (randomValue < option.Value)
            {
                return option.Key;
            }
            randomValue -= option.Value;
        }

        return modelOptions.Keys.First();
    }
    
    public override void Init(Bot bot) {
        _bot = (bot.ChatBot)bot;

        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }
    }
}