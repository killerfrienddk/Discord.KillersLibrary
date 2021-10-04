using System.Threading.Tasks;
using System;
using Discord.Rest;

namespace Interaction.TestingGrounds.Parameter {
	public interface IEventParam {
		DateTimeOffset Timestamp { get; }
		Task<RestAuditLogEntry> AuditLogEntryTask { get; }
	}
}