using Discord;

namespace Interaction.TestingGrounds.Parameter {
	public interface IGuildEventParam : IEventParam {
		IGuild Guild { get; set; }

		ulong GuildId { get; set; }
	}
}