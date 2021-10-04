using Discord.WebSocket;
using Discord;

namespace Interaction.TestingGrounds.Parameter {
	public interface IInteractionParams : IEventParam {
		ISocketMessageChannel Channel { get; }
		SocketUser User { get; }
		InteractionType Type { get; }
		string Token { get; }
		IDiscordInteractionData Data { get; }
		bool IsValidToken { get; }
	}
}