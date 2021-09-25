using System.Threading.Tasks;

namespace Interaction.Modules.Interaction {
    //An option for the IInteractionListener.
    public interface IInteractionListenerOption {
        //The custom id for the component this option is for.
        string CustomId { get; }

        //The function this option represents.
        InteractionHandler Function { get; }

        //Method that runs the function on the given input.
        Task<IInteractionResult> RunAsync(IInteractionEventParameters par);
    }
}