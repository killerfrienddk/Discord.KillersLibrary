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
		An option for the IInteractionListener.
	*/
	public class InteractionListenerOption : IInteractionListenerOption {
		/*
			The custom id for the component this option is for.
		*/
		public string CustomId { get; }
		
		/*
			The function this option represents.
		*/
		public InteractionHandler Function { get; }


		public InteractionListenerOption(string CustomId, InteractionHandler Function) {
			Preconditions.NotNull(CustomId, nameof(CustomId));
			Preconditions.NotNull(Function, nameof(Function));

			this.CustomId = CustomId;
			this.Function = Function;
		}


		
		// Method that runs the function on the given input.
		public async Task<IInteractionResult> RunAsync(IInteractionEventParameters par) {
			return await Function(par);
		}

		public override string ToString()
			=> $"{CustomId}";
	}
}