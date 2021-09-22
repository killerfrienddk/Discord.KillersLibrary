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
		Delegate that represents an action to be commited upon an interaction being activated.

		Takes in information about the reaction and its context.
		Returns an IReactionResult object that informs how the execution went and if the listener should be removed now.
	*/
	public delegate Task<IInteractionResult> InteractionHandler(IInteractionEventParameters parameters);


	public delegate Task<IInteractionResult> InteractionHandlerWithEmbedder(IInteractionEventParameters parameters, EmbedBuilder embed);

	public delegate Task<IUserMessage> MessageSender(Embed embed, MessageComponent component);


}