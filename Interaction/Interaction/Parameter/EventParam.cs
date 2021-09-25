using System.Threading.Tasks;
using System;
using Discord.Rest;
using Discord;



namespace Interaction.Types {
	public abstract class EventParam : IGuildEventParam, IEventParam {
		public IGuild Guild { get; set; }
		public GuildConfig GuildConfig { get; set; }
		public GuildSummary GuildSummary { get; set; }
		public ulong GuildId { get; set; }

		public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
		public Task<RestAuditLogEntry> AuditLogEntryTask { get; set; } = Task.FromResult<RestAuditLogEntry>(null); // new Task<RestAuditLogEntry>(() => null);
	}
}