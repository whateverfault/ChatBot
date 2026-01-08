using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.text_generator;

public class TextGeneratorEvents : ServiceEvents {
    private TextGeneratorService _textGenerator = null!;
    private StreamStateCheckerService _streamStateChecker = null!;
    
    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        _textGenerator = (TextGeneratorService)service;
        _streamStateChecker = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
        base.Init(service);
    }
    
    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        _streamStateChecker.OnStreamStateChanged += TrainAiWrapper;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _streamStateChecker.OnStreamStateChanged -= TrainAiWrapper;
    }

    private void TrainAiWrapper(StreamState streamState, StreamData? data) {
        if (streamState.WasOnline) {
            return;
        }
        
        _textGenerator.Train();
    }
}