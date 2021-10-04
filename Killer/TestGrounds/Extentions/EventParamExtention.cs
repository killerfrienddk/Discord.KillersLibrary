using System.Threading.Tasks;

using Discord;

using Interaction.TestingGrounds.Parameter;

namespace Interaction.TestingGrounds.Extensions {
	public static class EventParamExtention {
		public static async Task<bool> SetGuild(this IGuildEventParam par, IGuild guild) {
			await Task.CompletedTask;

			if(guild != null) {
				par.Guild = guild;
				par.GuildId = guild.Id;
				return true;
			}

			return false;
		}
	}
}