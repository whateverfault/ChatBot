using ChatBot.api.basic;
using ChatBot.bot.interfaces;
using ChatBot.bot.shared.handlers;

namespace ChatBot.bot.services.moderation.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Enabled);

    public readonly SafeField<List<ModAction>> ModerationActions = new SafeField<List<ModAction>>(
                                                                                                  [
                                                                                                      new ModAction(
                                                                                                                    "Level Requests",
                                                                                                                    0,
                                                                                                                    600,
                                                                                                                    "Реквесты только для випов и выше",
                                                                                                                    2,
                                                                                                                    Restriction.Vip,
                                                                                                                    isDefault: true,
                                                                                                                    clearAfterStream: true
                                                                                                                   ),
                                                                                                  ]
                                                                                                 );

    public readonly SafeField<List<WarnedUser>> WarnedUsers = new SafeField<List<WarnedUser>>([]);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        List<ModAction> actions,
        List<WarnedUser> warnedUsers) {
        ServiceState.Value = serviceState;
        ModerationActions.Value = actions;
        WarnedUsers.Value = warnedUsers;
    }
}