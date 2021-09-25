using System.Threading.Tasks;

namespace Interaction.Modules.Interaction {
    //An option for the IInteractionListener.
    public class InteractionListenerOption : IInteractionListenerOption {
		//The custom id for the component this option is for.
		public string CustomId { get; }
		
		//The function this option represents.
		public InteractionHandler Function { get; }

		public InteractionListenerOption(string CustomId, InteractionHandler Function) {
			Preconditions.NotNull(CustomId, nameof(CustomId));
			Preconditions.NotNull(Function, nameof(Function));

			this.CustomId = CustomId;
			this.Function = Function;
		}
		
		//Method that runs the function on the given input.
		public async Task<IInteractionResult> RunAsync(IInteractionEventParameters par) {
			return await Function(par);
		}

		public override string ToString()
			=> $"{CustomId}";
	}
}