using Discord;

namespace Interaction.Types {
	public interface IGuildEventParam : IEventParam {
		IGuild Guild { get; set; }
		GuildConfig GuildConfig { get; set; }
		GuildSummary GuildSummary { get; set; }

		ulong GuildId { get; set; }
	}
}