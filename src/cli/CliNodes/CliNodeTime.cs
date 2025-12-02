using ChatBot.bot.shared.handlers;

namespace ChatBot.cli.CliNodes;

public delegate long TimeGetter();
public delegate void TimeSetter(long value);

public class CliNodeTime : CliNode {
    private readonly TimeGetter _getter;
    private readonly TimeSetter? _setter;
    private readonly CliNodePermission _permission;

    private readonly bool _isUnixEpoch;
    
    protected override string Text { get; }


    public CliNodeTime(string text, TimeGetter getter, CliNodePermission permission, TimeSetter? setter = null, bool isUnixEpoch = false) {
        Text = text;
        _getter = getter;
        _permission = permission;
        _isUnixEpoch = isUnixEpoch;
        
        if (setter != null) {
            _setter = setter;
        }
    }

    public override int PrintValue(int index, out string end) {
        base.PrintValue(index, out end);
        var parsedGetter = ParseGetter(_getter.Invoke());
        IoHandler.Write($" - {parsedGetter}");
        return 0;
    }

    public override void Activate(CliState state) {
        if (_permission == CliNodePermission.ReadOnly) return;

        var parsedGetter = ParseGetter(_getter.Invoke());
        IoHandler.WriteLine($"Value: {parsedGetter}");

        var line = IoHandler.ReadLine("New Value: ");
        if (string.IsNullOrEmpty(line)) {
            return;
        }

        var parsedSetter = ParseSetter(line);
        if (parsedSetter < 0) return;
        
        _setter?.Invoke(parsedSetter);
    }

    private string ParseGetter(long time) {
        if (_isUnixEpoch) {
            var unixEpochTime = DateTimeOffset
                               .FromUnixTimeSeconds(time)
                               .ToLocalTime();
            return unixEpochTime.ToString("G");
        }

        var timeSpan = TimeSpan.FromSeconds(time);
        var hoursLength = timeSpan.Hours < 100?
                              2 :
                              timeSpan.Hours.ToString().Length;
        return $"{timeSpan.Hours.ToString($"D{hoursLength}")}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
    }

    private long ParseSetter(string time) {
        if (_isUnixEpoch) {
            if (!DateTimeOffset.TryParse(time, out var unixEpochTime)) {
                HandleWrongFormat();
                return -1;
            }

            return unixEpochTime.ToUnixTimeSeconds();
        }
        if (!TimeSpan.TryParse(time, out var classicTime)) {
            HandleWrongFormat();
            return -1;
        }
            
        return (long)classicTime.TotalSeconds;
    }

    private void HandleWrongFormat() {
        IoHandler.WriteLine("Wrong format.");
        IoHandler.ReadKey(true);
        IoHandler.Clear();
    }
}