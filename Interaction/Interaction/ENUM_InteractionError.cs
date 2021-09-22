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
		The type of error that occured when handling a reaction event
	*/
	public enum InteractionError {
		Unsuccessful = 1,	// Handler was ran, but was unsuccessful
		Exception,			// Exception occured in handler
		BadInput,			// The input was invalid
		InvalidUser,		// The issuer is not allowed to do this command
		TooFast,			// User needs to wait longer if they want to issue that command
		Expired,			// The listener has expired
	}
}