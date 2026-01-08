using ChatBot.bot.shared.handlers;
using ChatBot.cli.data.rendering;

namespace ChatBot.cli;

public enum Renderer{
    Old,
    New,
}

public class CliRenderer {
    private readonly object _lock = new object();
    
    private ICliRenderer? _renderer;
    private CliState? _state;

    private bool _forcedToRender;

    private CliRendererOptions Options { get; } = new CliRendererOptions();
    

    public void Bind(CliState state) {
        lock (_lock) {
            _state = state;

            Options.Load();
            SetRenderer(Options.CurrentRenderer);
        }
    }

    public int GetRendererAsInt() {
        lock (_lock) {
            return (int)Options.CurrentRenderer;
        }
    }
    
    public void RendererNext() {
        lock (_lock) {
            if (Options.CurrentRenderer == Renderer.Old) {
                Options.SetRenderer(Renderer.New);
                SetRenderer(Options.CurrentRenderer);
                return;
            }
            
            Options.SetRenderer(Renderer.Old);
            SetRenderer(Options.CurrentRenderer);
        }
    }

    private void SetRenderer(Renderer renderer) {
        if (renderer == Renderer.New) {
            _renderer = new CliNewRenderer();
            return;
        }
        
        _renderer = new CliOldRenderer();
    }
    
    public Task Start() {
        if (_state == null) return Task.CompletedTask;
        RenderNodes();

        return Task.Run(() => {
                            lock (_lock) {
                                while (true) {
                                    if (!_forcedToRender) {
                                        _renderer?.HandleInput(_state);
                                    }

                                    IoHandler.Clear();
                                    RenderNodes();
                                    _forcedToRender = false;
                                }
                            }
                        });
    }

    public void ForceToRender() {
        _forcedToRender = true;
    }
    
    private void RenderNodes() {
        if (_state == null) return;
        
        _renderer?.Render(_state);
    }
}