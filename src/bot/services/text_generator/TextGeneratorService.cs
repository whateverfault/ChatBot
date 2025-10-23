using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_logs;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;

namespace ChatBot.bot.services.text_generator;

public class TextGeneratorService : Service {
    private static TwitchChatBot Bot => TwitchChatBot.Instance;
    private static ITwitchClient? Client => Bot.GetClient();
    
    private readonly Random _random = new Random();

    public override string Name => ServiceName.TextGenerator;
    public override TextGeneratorOptions Options { get; } = new TextGeneratorOptions();


    public void Train() {
        var chatLogsService = (ChatLogsService)ServiceManager.GetService(ServiceName.ChatLogs);
        var chatLogs = chatLogsService.GetLogs();
        if (chatLogs.Count == 0) return;

        foreach (var message in chatLogs) {
            TrainOnMessage(message.Text);
        }
        Options.Save();
    }
    
    private void TrainOnMessage(string message) {
        var words = message.Split([' ', '\n', '\r', '\t',], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length <= Options.ContextSize) return;

        for (var i = 0; i < words.Length - Options.ContextSize; i++) {
            var context = string.Join(" ", words.Skip(i).Take(Options.ContextSize));
            var nextWord = words[i + Options.ContextSize];

            if (!Options.Model.ContainsKey(context)) {
                Options.Model[context] = new Dictionary<string, int>();
            }

            if (!Options.Model[context].ContainsKey(nextWord)) {
                Options.Model[context][nextWord] = 0;
            }

            Options.Model[context][nextWord]++;
        }
    }

    public ErrorCode Generate(out string message, string? input = null) {
        message = string.Empty;
        
        if (Options.ServiceState == State.Disabled) return ErrorCode.ServiceDisabled;
        if (Options.Model.Count == 0) return ErrorCode.InvalidData;

        string context;
        var result = new List<string>();

        if (!string.IsNullOrEmpty(input)) {
            var inputWords = input.Split([' ', '\n', '\r', '\t',], StringSplitOptions.RemoveEmptyEntries);

            if (inputWords.Length >= Options.ContextSize) {
                var lastWords = inputWords.TakeLast(Options.ContextSize).ToArray();
                context = string.Join(" ", lastWords);
                result.AddRange(inputWords);
            }
            else {
                context = string.Join(" ", inputWords);
                result.AddRange(inputWords);

                if (inputWords.Length > 0) {
                    var matchingContexts = Options.Model.Keys
                                                  .Where(key => key.StartsWith(context))
                                                  .ToList();

                    context = matchingContexts.Count > 0? 
                                  matchingContexts[_random.Next(matchingContexts.Count)] :
                                  Options.Model.Keys.ElementAt(_random.Next(Options.Model.Keys.Count));
                }
                else {
                    context = Options.Model.Keys.ElementAt(_random.Next(Options.Model.Keys.Count));
                }

                result = context.Split(' ').ToList();
            }
        }
        else {
            context = Options.Model.Keys.ElementAt(_random.Next(Options.Model.Keys.Count));
            result = new List<string>(context.Split(' '));
        }

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
        if (Client == null) return;
        
        if (Options.ServiceState == State.Disabled) {
            ErrorHandler.LogError(ErrorCode.ServiceDisabled);
            return;
        }

        var err = Generate(out var message);
        if (ErrorHandler.LogError(err)) return;
        
        Client.SendMessage(message);
    }
    
    private string WeightedRandomChoice(Dictionary<string, int> model) {
    var totalWeight = model.Sum(x => x.Value);
    var randomValue = _random.Next(totalWeight);
    var currentWeight = 0;
    
    foreach (var option in model) {
        currentWeight += option.Value;
        if (randomValue < currentWeight) {
            return option.Key;
        }
    }
    
    return model.Keys.First();
    }
}