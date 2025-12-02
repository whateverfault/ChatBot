using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.casino.data;
using ChatBot.bot.services.casino.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.casino;

public class CasinoOptions : Options{
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "casino";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    
    public float RandomValue => _saveData!.RandomValue;
    public float BaseMultiplier => _saveData!.BaseMultiplier;
    public float AdditionalMultiplier => _saveData!.AdditionalMultiplier;

    private List<Duel> Duels => _saveData!.Duels;
    
    
    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
        }
    }
    
    public override void SetDefaults() {
        _saveData = new SaveData();
        NewRandomValue();
        Save();
    }
    
    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }
    
    public void NewRandomValue() {
        _saveData!.RandomValue = Random.Shared.NextSingle();
        Save();
    }

    public bool AddDuel(string subject, string obj, long quantity) {
        if (ContainsDuel(subject, obj)) return false;
        Duels.Add(new Duel(subject, obj, quantity));
        
        Save();
        return true;
    }
    
    public bool RemoveDuel(string obj, string? subject = null) {
        for (var i = 0; i < Duels.Count; ++i) {
            var duel = Duels[i];
            
            if (string.IsNullOrEmpty(subject)) {
                if (!duel.Object.Equals(obj)) continue;

                Duels.RemoveAt(i);
                Save();
                return true;
            }
            
            if (!(duel.Object.Equals(obj) && duel.Subject.Equals(subject))) continue;
            
            Duels.RemoveAt(i);
            Save();
            return true;
        }
        
        return false;
    }
    
    public void RemoveDuels(string subject) {
        for (var i = 0; i < Duels.Count; ++i) {
            var duel = Duels[i];
            
            if (!duel.Subject.Equals(subject)) continue;
            Duels.RemoveAt(i);
        }
        
        Save();
    }
    
    public Duel? GetDuel(string obj, string? subject = null) {
        return string.IsNullOrEmpty(subject)? 
                   Duels.FirstOrDefault(duel => duel.Object.Equals(obj)) :
                   Duels.FirstOrDefault(duel => duel.Subject.Equals(subject) && duel.Object.Equals(obj));
    }

    public List<Duel> GetDuels(string obj) {
        return Duels.Where(duel => duel.Object.Equals(obj)).ToList();
    }
    
    private bool ContainsDuel(string subject, string obj) {
        return Duels.Any(duel => duel.Subject.Equals(subject) && duel.Object.Equals(obj));
    }
}