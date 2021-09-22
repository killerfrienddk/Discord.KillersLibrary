using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

// ---- //

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

// ---- //

using csharpi;
using csharpi.Types;
using csharpi.Services;
using csharpi.Sample;  
using csharpi.Processed;
using csharpi.Test;
using csharpi.Globals;
using csharpi.Fun;



namespace csharpi.Modules.Interaction {
	
	/*
		Represents the results of running an InteractionHandler.
	*/
	public class InteractionResult : IInteractionResult {

		//	Gets the exception that may have occurred during the handler execution.
		public Exception Exception { get; }

		//	Describes the error type that may have occurred during the operation.
		public InteractionError? Error { get; }

		//	Describes the reason for the error.
		public string ErrorReason { get; }


		//	Indicates whether the operation was successful or not.
		public bool IsSuccess => !Error.HasValue;

		//	Indicates whether the listener has completed its purpose and should be removed.
		public bool IsComplete { get; set; }

		//	Indicates that the message should be updated.
		public bool ShouldUpdate { get; set; } = true;

		/*
			Indicates that the message update should be forced before we return.

			Has no effect if ShouldUpdate is false.
			If false, the update will be scheduled to happen.
			Used for when the listener might be triggered too often to update each time.
		*/
		public bool ForceUpdate { get; set; } = true;

		//	Indicates that the listener should save the id of this component for later.
		public bool ShouldSaveComponentId { get; set; }


		//	Builder for the new embed.
		public EmbedBuilder Embed { get; set; }

		//	Builder for the new components.
		public ComponentBuilder Components { get; set; }


		// ---- //


		/*
			Private constructor for this class.
		*/
		private InteractionResult(
			Exception exception = null, InteractionError? error = null, string errorReason = null, 
			bool isComplete = false, bool shouldUpdate = true, bool forceUpdate = true, bool shouldSaveComponentId = true, 
			EmbedBuilder embed = null, ComponentBuilder components = null
		) {
			Exception = exception;
			Error = error;
			ErrorReason = errorReason;
			IsComplete = isComplete;
			ShouldUpdate = shouldUpdate;
			ForceUpdate = forceUpdate;
			ShouldSaveComponentId = shouldSaveComponentId;
			Embed = embed;
			Components = components;
		}



		// ---- //


		/*
			Creates a new InteractionResult that indicates successful execution.
		*/
		public static InteractionResult FromSuccess(
			bool isComplete = false, bool shouldUpdate = true, bool forceUpdate = true, bool shouldSaveComponentId = true, 
			EmbedBuilder embed = null, ComponentBuilder components = null
		)
            => new InteractionResult(
				isComplete:isComplete, shouldUpdate:shouldUpdate, forceUpdate:forceUpdate, shouldSaveComponentId:shouldSaveComponentId, 
				embed:embed, components:components
			);


		/*
			Creates a new InteractionResult that indicates failiure.
		*/
		public static InteractionResult FromError(
			InteractionError error, string reason, 
			bool isComplete = false, bool shouldUpdate = false, bool forceUpdate = false, bool shouldSaveComponentId = false, 
			EmbedBuilder embed = null, ComponentBuilder components = null
		)
            => new InteractionResult(
				null, error:error, errorReason:reason, 
				isComplete:isComplete, shouldUpdate:shouldUpdate, forceUpdate:forceUpdate, shouldSaveComponentId:shouldSaveComponentId, 
				embed:embed, components:components
			);


		/*
			Creates a new InteractionResult that indicates failiure.
		*/
		public static InteractionResult FromError(Exception ex, bool isComplete = true)
            => new InteractionResult(
				ex, InteractionError.Exception, ex.Message, 
				isComplete, shouldUpdate:false, forceUpdate:false, shouldSaveComponentId:false
			);


		/*
			Creates a new InteractionResult that indicates failiure.
		*/
		public static InteractionResult FromResult(IInteractionResult result)
            => new InteractionResult(
				exception:result.Exception, error:result.Error, errorReason:result.ErrorReason, 
				isComplete:result.IsComplete, shouldUpdate:result.ShouldUpdate, forceUpdate:result.ForceUpdate, shouldSaveComponentId:result.ShouldSaveComponentId, 
				embed:result.Embed, components:result.Components
			);


		// ---- //


        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

	}
}