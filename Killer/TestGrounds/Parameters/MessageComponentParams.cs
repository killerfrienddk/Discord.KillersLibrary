using Discord.WebSocket;
using Discord;



namespace Interaction.TestingGrounds.Parameter {
	public class MessageComponentParams : InteractionParams {
		public SocketMessageComponent MessageComponent { get; set; }

		public new SocketMessageComponentData Data => MessageComponent.Data;
		public SocketUserMessage Message => MessageComponent.Message;
		public IMessageComponent Component { get; set; }
	}
}