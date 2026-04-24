using ChatBot.bot.services.Static;
using Newtonsoft.Json;

namespace ChatBot.bot.services.casino.data;

public class GambleEmote {
    private static readonly CasinoService _service = (CasinoService)Services.Get(ServiceId.Casino);
    
    [JsonProperty("text")]
    public string Name;
    
    [JsonProperty("part")]
    public double Part;
    
    [JsonProperty("combo_coefficient")]
    public double ComboCoefficient;
    
    [JsonProperty("bonus_coefficient")]
    public double BonusCoefficient;
    
    [JsonProperty("combo")]
    public int Combo;
    
    
    public GambleEmote(string name) {
        Name = name;
        Part = 1;
        ComboCoefficient = 1;
        BonusCoefficient = 0.25;
        Combo = 3;
    }
    
    [JsonConstructor]
    public GambleEmote(
        [JsonProperty("text")] string name,
        [JsonProperty("part")] double part,
        [JsonProperty("combo_coefficient")] double comboCoefficient,
        [JsonProperty("bonus_coefficient")] double bonusCoefficient,
        [JsonProperty("combo")] int combo) {
        Name = name;
        Part = part;
        ComboCoefficient = comboCoefficient;
        BonusCoefficient = bonusCoefficient;
        Combo = combo;
    }

    public void SetText(string value) {
        Name = value;
        _service.Options.Save();
    }

    public string GetText() {
        return Name;
    }
    
    public void SetPart(double part) {
        Part = part;
        _service.Options.Save();
    }

    public double GetPart() {
        return Part;
    }
    
    public void SetComboCoefficient(double value) {
        ComboCoefficient = value;
        _service.Options.Save();
    }

    public double GetComboCoefficient() {
        return ComboCoefficient;
    }
    
    public void SetBonusCoefficient(double value) {
        BonusCoefficient = value;
        _service.Options.Save();
    }

    public double GetBonusCoefficient() {
        return BonusCoefficient;
    }
    
    public void SetAmountForCombo(int value) {
        Combo = value;
        _service.Options.Save();
    }

    public int GetAmountForCombo() {
        return Combo;
    }
}