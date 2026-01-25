using System.Text;
using ChatBot.bot.services.chat_commands.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.localization.data;

namespace ChatBot.bot.services.localization;

public class LocalizationService : Service {
    public override LocalizationOptions Options { get; } = new LocalizationOptions();

    public Lang Language => Options.Language;
    

    public string GetStr(StrId id, params object[] args) {
        return GetStr(id, Language, args);
    }

    public string GetCmdDescStr(ChatCommand cmd) {
        var strId = Enum.IsDefined(typeof(StrId), $"Cmd{cmd.Id}")
                        ? Enum.Parse<StrId>($"Cmd{cmd.Id}")
                        : StrId.InvalidStrId;
        
        return GetStr(strId);
    }
    
    private string GetStr(StrId id, Lang lang, params object[] args) {
        var localization = LocalizationsList.GetLocalization(id);
        if (localization == null)
            return string.Empty;
        
        return FormatLocalization(localization, lang, args);
    }

    private string FormatLocalization(Localization localization, Lang lang, params object[] args) {
        var fmt = localization.Variants[lang];
        
        if (args.Length <= 0 && localization.Args.Length > 0)
            return FormatString(fmt, localization.Args.Select(x => x.Invoke()).ToArray());

        return FormatString(fmt, args);
    }
    
    private string FormatString(string fmt, params object[] args) {
        if (args.Length <= 0)
            return fmt;
        
        var sb = new StringBuilder();
        var argIndex = 0;
        
        for (var i = 0; i < fmt.Length; i++) {
            if (fmt[i] == '\\') {
                ++i;
                continue;
            }
            
            if (i < fmt.Length - 1 
             && argIndex < args.Length
             && fmt[i] == '{'
             && fmt[++i] == '}') {
                sb.Append(args[argIndex++]);
                continue;
            }

            sb.Append(fmt[i]);
        }

        return sb.ToString();
    }
    
    public int GetLanguageAsInt() {
        return (int)Options.Language;
    }

    public void LanguageNext() {
        Options.SetLanguage((Lang)(((int)Options.Language + 1) % Enum.GetValues(typeof(Lang)).Length));
    }
}