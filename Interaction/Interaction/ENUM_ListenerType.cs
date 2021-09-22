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
using System.ComponentModel;

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
		The type of listener it is.
	*/
	public enum ListenerType {
		
		[Description("The default listener type.")]
		Default = 0,

		[Description("The listener is owned by a user and can only be used by them.")]
		Owned,

		[Description("The listener is hosted by a user, but can be used by anyone. Ex.: Polls")]
		Hosted,
	}
}