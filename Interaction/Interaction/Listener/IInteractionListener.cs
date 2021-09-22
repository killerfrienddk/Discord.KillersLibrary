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
		Listenes to reactions added to / removed from a message.	
	*/
	public interface IInteractionListener {

		/*
			The type of listener this is.
		*/
		ListenerType ListenerType { get; }

		TimeSpan MaxLifespan { get; } 
		TimeSpan MaxIdle { get; }
		TimeSpan Cooldown { get; }
	
		DateTimeOffset Timestamp { get; } 
		DateTimeOffset LastAction { get; }

		IUserMessage Message { get; }
		IMessageChannel Channel { get; }
		
		IUser Target { get; }
		IUser Owner { get; }

		InteractionListenerService Service { get; }
		ListenerStatus Status { get; }

		DateTimeOffset Deadline { get; }



		/*
			Checks if the listener has expired.
		*/
		bool IsExpired();


		
		/*
			Runs when a reaction event has been fired.
		*/
		Task<IInteractionResult> RunAsync(IInteractionEventParameters par);


		/*
			Connects the listener to a ReactionListenerService
		*/
		Task<bool> Connect(InteractionListenerService service);

		
		/*
			Kills off the listener.
		*/
		Task<bool> Kill();

	}
}