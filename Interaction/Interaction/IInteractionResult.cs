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
		Represents the results of running a reaction handler.
	*/
	public interface IInteractionResult {


		/*
			Gets the exception that may have occurred during the handler execution.
		*/
		Exception Exception { get; }

		/*
			Describes the error type that may have occurred during the operation.
		*/
		InteractionError? Error { get; }

		/*
			Describes the reason for the error.
		*/
		string ErrorReason { get; }

		/*
			Indicates whether the operation was successful or not.
		*/
		bool IsSuccess { get; }

		/*
			Indicates whether the listener has completed its purpose and should be removed.
		*/
		bool IsComplete { get; }

		/*
			Indicates that the message should be updated.
		*/
		bool ShouldUpdate { get; }

		/*
			Indicates that the message update should be forced before we return.

			Has no effect if ShouldUpdate is false.
			If false, the update will be scheduled to happen.
			Used for when the listener might be triggered too often to update each time.
		*/
		bool ForceUpdate { get; }

		/*
			Indicates that the listener should save the id of this component for later.
		*/
		bool ShouldSaveComponentId { get; }


		/*
			Builder for the new embed.
		*/
		EmbedBuilder Embed { get; }

		/*
			Builder for the new components.
		*/
		ComponentBuilder Components { get; }

	}
}