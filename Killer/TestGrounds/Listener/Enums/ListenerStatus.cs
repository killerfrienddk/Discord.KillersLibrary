using System.ComponentModel;

namespace Interaction.TestingGrounds.Listener.Enums {
    //The current status of a listener.
    public enum ListenerStatus {
        [Description("Initially.")]
        SettingUp,
        [Description("When set up and ready.")]
        Started,
        [Description("When the listener has completed it's role.")]
        Completed,
        [Description("When the listener successfully been killed.")]
        Dead,
    }
}