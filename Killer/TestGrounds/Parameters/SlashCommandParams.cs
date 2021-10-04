using Discord.WebSocket;

namespace Interaction.TestingGrounds.Parameter {
	public class SlashCommandParams : InteractionParams {
		public SocketSlashCommand SlashCommand { get; set; }

		public new SocketSlashCommandData Data => SlashCommand.Data;
	}
}