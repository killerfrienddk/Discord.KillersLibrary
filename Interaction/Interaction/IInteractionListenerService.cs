using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Timers;

// ---- //

using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Discord.Rest;

// ---- //

using csharpi;
using csharpi.Types;
using csharpi.Services;
using csharpi.Sample;
using csharpi.Globals;


using Timer = System.Timers.Timer;


namespace csharpi.Modules.Interaction {

	/*
		Service that manages what to do when interactions occur.
	*/
	public interface IInteractionListenerService {

	}
}