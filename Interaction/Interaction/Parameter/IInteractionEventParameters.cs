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
		The parameters we send to a reaction listener when it is invoked.
	*/
	public interface IInteractionEventParameters {

		/*
			The custom id of the component.
		*/
		string CustomId { get; }

		/*
			The type of component that was used.
		*/
		ComponentType Type { get; }

		/*
			The component that was used.
		*/
		IMessageComponent Component { get; }


		/*
			The values of the component.
		*/
		IReadOnlyCollection<string> Values { get; }



		/*
			The user that did the interaction.
		*/
		IUser User { get; }

		/*
			The guild this happened in.
		*/
		IGuild Guild { get; }

		/*
			The channel the message belongs to.
		*/
		IMessageChannel Channel { get; }

		/*
			The message this happened to.
		*/
		IMessage Message { get; }


		/*
			The time this occured in.
		*/
		DateTimeOffset Timestamp { get; }

		/*
			The interaction object.
		*/
		SocketMessageComponent Interaction { get; }
	}
}