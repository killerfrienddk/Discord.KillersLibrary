using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using Discord.WebSocket;
using Discord.Rest;
using Discord;

namespace KillersLibraryTestBot.Services {
    public class UserService {
        public async Task<List<RestGuildUser>> GetSortedUserListAsync(SocketGuild guild, bool bots = false) {
            List<RestGuildUser> restGuildUsers = new();

            IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> src = guild.GetUsersAsync();
            IAsyncEnumerator<IReadOnlyCollection<IGuildUser>> e = src.GetAsyncEnumerator();
            try {
                while (await e.MoveNextAsync()) {
                    foreach (RestGuildUser user in e.Current) {
                        if (!bots) if (user.IsBot || user.IsWebhook) continue;

                        restGuildUsers.Add(user);
                    }
                }
            } finally { if (e != null) await e.DisposeAsync(); }

            return restGuildUsers.OrderBy(r => r.Nickname ?? r.Username).ToList();
        }
    }
}
