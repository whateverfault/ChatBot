using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.shop.data;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.shop;

public class ShopOptions : Options {
    private readonly object _fileLock = new object();
    private SaveData? _saveData;
    
    protected override string Name => "shop";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public List<ShopLot> Lots => _saveData!.Lots;
    
    
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
        Save();
    }
    
    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public bool AddLot(ShopLot lot) {
        if (GetLot(lot.Name) != null) return false;
        
        Lots.Add(lot);
        Save(); return true;
    }
    
    public bool RemoveLot(string lotName) {
        var lot = GetLot(lotName);
        if (lot == null) return false; 
        
        for (var i = 0; i < Lots.Count; ++i) {
            if (Lots[i].Id == lot.Id) {
                Lots.RemoveAt(i);
                Save(); return true;
            }
        }
        return false;
    }
    
    public ShopLot? GetLot(string name) {
        return Lots.FirstOrDefault(lot => lot.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
    
    public ShopLot? GetLot(int id) {
        return Lots.FirstOrDefault(lot => lot.Id == id);
    }
}