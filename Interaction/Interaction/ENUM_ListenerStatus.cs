namespace Interaction.Modules.Interaction {
    /*
		The current status of a listener
	*/
    public enum ListenerStatus {
		SettingUp,			// Initially
		Started,			// When set up and ready
		Completed,			// When the listener has completed it's role
		Dead,				// When the listener successfully been killed
	}
}