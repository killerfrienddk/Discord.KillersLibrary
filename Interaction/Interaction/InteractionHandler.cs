using System.Threading.Tasks;
using Discord;


namespace Interaction.Modules.Interaction {

    /*
		Delegate that represents an action to be commited upon an interaction being activated.

		Takes in information about the reaction and its context.
		Returns an IReactionResult object that informs how the execution went and if the listener should be removed now.
	*/
    public delegate Task<IInteractionResult> InteractionHandler(IInteractionEventParameters parameters);


	public delegate Task<IInteractionResult> InteractionHandlerWithEmbedder(IInteractionEventParameters parameters, EmbedBuilder embed);

	public delegate Task<IUserMessage> MessageSender(Embed embed, MessageComponent component);


}