using System;
using Discord;

namespace Interaction.Modules.Interaction {
    //Represents the results of running a reaction handler.
    public interface IInteractionResult {
        //Gets the exception that may have occurred during the handler execution.
        Exception Exception { get; }

        //Describes the error type that may have occurred during the operation.
        InteractionError? Error { get; }

        //Describes the reason for the error.
        string ErrorReason { get; }

        //Indicates whether the operation was successful or not.
        bool IsSuccess { get; }

        //Indicates whether the listener has completed its purpose and should be removed.
        bool IsComplete { get; }

        //Indicates that the message should be updated.
        bool ShouldUpdate { get; }

        //Indicates that the message update should be forced before we return.

        //Has no effect if ShouldUpdate is false.
        //If false, the update will be scheduled to happen.
        //Used for when the listener might be triggered too often to update each time.
        bool ForceUpdate { get; }

        //Indicates that the listener should save the id of this component for later.
        bool ShouldSaveComponentId { get; }

        //Builder for the new embed.
        EmbedBuilder Embed { get; }

        //Builder for the new components.
        ComponentBuilder Components { get; }
    }
}