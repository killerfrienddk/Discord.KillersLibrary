using System.Threading.Tasks;
using System;
using Discord.Rest;

namespace Interaction.Types {
	public interface IEventParam {
		DateTimeOffset Timestamp { get; }
		Task<RestAuditLogEntry> AuditLogEntryTask { get; }
	}
}