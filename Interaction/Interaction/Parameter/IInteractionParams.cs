using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Discord.WebSocket;
using Discord;

namespace Interaction.Types {
	public interface IInteractionParams : IEventParam {
		ISocketMessageChannel Channel { get; }
		SocketUser User { get; }
		InteractionType Type { get; }
		string Token { get; }
		IDiscordInteractionData Data { get; }
		bool IsValidToken { get; }
	}
}