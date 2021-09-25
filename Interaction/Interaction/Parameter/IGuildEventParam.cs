using Discord;

namespace Interaction.Types {
	public interface IGuildEventParam : IEventParam {
		IGuild Guild { get; set; }

		ulong GuildId { get; set; }
	}
}