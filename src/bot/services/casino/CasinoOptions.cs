using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.casino.data;
using ChatBot.bot.services.casino.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.casino;

public class CasinoOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "casino";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    
    public double RandomValue => _saveData!.RandomValue;
    public float BaseCoefficient => _saveData!.BaseCoefficient;
    public float AdditionalCoefficient => _saveData!.AdditionalCoefficient;

    public IReadOnlyList<GambleEmote> Emotes => _saveData!.Emotes;
    public int EmoteSlots => _saveData!.EmoteSlots;
    
    private List<Duel> Duels => _saveData!.Duels;

    public event EventHandler<GambleEmote>? OnEmoteAdded;
    public event EventHandler<string>? OnEmoteRemoved;
    
    
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
        _saveData!.RandomValue = Random.Shared.NextDouble();
        Save();
    }

    public bool AddDuel(string subject, string obj, double quantity) {
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
            
            if (duel.Object != obj || duel.Subject != subject) continue;
            
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

    public int GetEmoteSlots() {
        return EmoteSlots;
    }
    
    public void SetEmoteSlots(int value) {
        _saveData!.EmoteSlots = value;
        Save();
    }

    public float GetBaseCoefficient() {
        return BaseCoefficient;
    }
    
    public void SetBaseCoefficient(float value) {
        _saveData!.BaseCoefficient = value;
        Save();
    }
    
    public float GetAdditionalCoefficient() {
        return AdditionalCoefficient;
    }
    
    public void SetAdditionalCoefficient(float value) {
        _saveData!.AdditionalCoefficient = value;
        Save();
    }

    public bool AddEmote(string name) {
        if (Emotes.Any(x => x.Name == name)) {
            return false;
        }

        var emote = new GambleEmote(name);
        _saveData!.Emotes.Add(emote);
        Save();

        OnEmoteAdded?.Invoke(this, emote);
        return true;
    }

    public bool RemoveEmote(string name) {
        for (var i = 0; i < Emotes.Count; ++i) {
            var emote = Emotes[i];
            if (emote.Name != name) {
                continue;
            }

            _saveData!.Emotes.RemoveAt(i);
            Save();
            
            OnEmoteRemoved?.Invoke(this, name);
            return true;
        }
        
        return false;
    }
    
    private bool ContainsDuel(string subject, string obj) {
        return Duels.Any(duel => duel.Subject == subject && duel.Object == obj);
    }
}