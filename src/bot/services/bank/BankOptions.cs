using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.bank.data;
using ChatBot.bot.services.bank.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.bank;

public class BankOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    private static string Name => "bank";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    public long MoneySupply => _saveData!.MoneySupply;
    
    private Dictionary<string, Account> Accounts => _saveData!.Accounts; 
    private Dictionary<string, long> Rewards => _saveData!.Rewards;
    
    
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
    
    private bool BankAdd(string userId, long quantity, bool gain) {
        if (string.IsNullOrEmpty(userId)) 
            return false;

        if (!Accounts.TryGetValue(userId, out var account)) {
            if (quantity < 0) return false;
            Accounts.Add(userId, new Account(userId, quantity));
        }

        return BankAdd(account, quantity, gain);
    }
    
    private bool BankAdd(Account? account, long quantity, bool gain) {
        if (account != null) {
            if (account.Money + quantity < 0) 
                return false;
            
            account.AddMoney(quantity, gain);
        }
        
        if (_saveData!.MoneySupply + quantity >= 0) 
            _saveData!.MoneySupply += quantity;
        
        Save();
        return true;
    }
    
    public bool Deposit(string userId, long quantity, bool gain = true) {
        if (quantity == 0) return true;
        return quantity > 0 && BankAdd(userId, quantity, gain);
    }

    public bool Deposit(Account? account, long quantity, bool gain = true) {
        if (quantity == 0) return true;
        return quantity > 0 && BankAdd(account, quantity, gain);
    }
    
    public bool TakeOut(string userId, long quantity, bool gain = true) {
        if (quantity == 0) return false;
        return quantity > 0 && BankAdd(userId, -quantity, gain);
    }

    public bool TakeOut(Account? account, long quantity, bool gain = true) {
        if (quantity == 0) return false;
        return quantity > 0 && BankAdd(account, -quantity, gain);
    }
    
    public bool GetBalance(string userId, out long balance) {
        balance = -1;
        if (!GetAccount(userId, out var gambler) || gambler == null) 
            return false;
        
        balance = gambler.Money;
        return true;
    }

    public bool DeleteAccount(string userId) {
        var result = Accounts.Remove(userId);

        Save();
        return result;
    }
    
    public bool GetAccount(string userId, out Account? account) {
        return Accounts.TryGetValue(userId, out account);
    }

    public Dictionary<string, Account> GetAccounts() {
        return Accounts;
    }
    
    public bool AddReward(string rewardId, long quantity) {
        return Rewards.TryAdd(rewardId, quantity);
    }
    
    public bool RemoveReward(string rewardId) {
        return Rewards.Remove(rewardId);
    }

    public bool GetReward(string rewardId, out long quantity) {
        return Rewards.TryGetValue(rewardId, out quantity);
    }
    
    public (string, long) GetReward(int index) {
        var reward = Rewards.ElementAtOrDefault(index);
        reward.Deconstruct(out var key, out var value);
        return (key, value);
    }
    
    public Dictionary<string, long> GetRewards() {
        return Rewards;
    }

    public void UpdateActivity(string userId) {
        if (!GetAccount(userId, out var account) || account == null)
            return;

        account.UpdateActivity();
        Save();
    }
}