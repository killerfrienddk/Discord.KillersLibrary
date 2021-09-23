namespace Interaction.Modules.Interaction {

    /*
		The type of error that occured when handling a reaction event
	*/
    public enum InteractionError {
		Unsuccessful = 1,	// Handler was ran, but was unsuccessful
		Exception,			// Exception occured in handler
		BadInput,			// The input was invalid
		InvalidUser,		// The issuer is not allowed to do this command
		TooFast,			// User needs to wait longer if they want to issue that command
		Expired,			// The listener has expired
	}
}