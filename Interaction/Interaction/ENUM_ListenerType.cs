using System.ComponentModel;

namespace Interaction.Modules.Interaction {
    //The type of listener it is.
    public enum ListenerType {
        [Description("The default listener type.")]
        Default = 0,
        [Description("The listener is owned by a user and can only be used by them.")]
        Owned,
        [Description("The listener is hosted by a user, but can be used by anyone. Ex.: Polls")]
        Hosted,
    }
}