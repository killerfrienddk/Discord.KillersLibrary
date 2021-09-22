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
	public interface IInteractionListenerOption {
		
		/*
			The custom id for the component this option is for.
		*/
		string CustomId { get; }
		
		/*
			The function this option represents.
		*/
		InteractionHandler Function { get; }
		
		
		/*
			Method that runs the function on the given input.
		*/
		Task<IInteractionResult> RunAsync(IInteractionEventParameters par);
	}
}