using ChatBot.api.basic;

namespace ChatBot.cli.data.rendering.saved;

internal class SaveDataDto {
    public SafeField<Renderer> CurrentRenderer = new SafeField<Renderer>(Renderer.New);


    public SaveDataDto(){}
    
    public SaveDataDto(Renderer renderer) {
        CurrentRenderer.Value = renderer;
    }
}