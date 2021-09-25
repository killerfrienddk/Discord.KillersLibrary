using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using Discord.Rest;
using Discord;

namespace Interaction.Utilities {
    public static partial class EmbedUtilities {
        public static EmbedBuilder StandardEmbed(string title, IUser author = null, IUser target = null)
            => StandardEmbed(new EmbedBuilder(), title, author, target);

        public static EmbedBuilder StandardEmbed(EmbedBuilder embed, string title, IUser author = null, IUser target = null) {
            embed ??= new EmbedBuilder();

            embed
                .WithTitle(title)
                .WithCurrentTimestamp()
                ;

            if (author != null) embed.WithAuthor(author);

            if (target != null) embed.WithFooter(footer => {
                footer.WithIconUrl(target.GetAvatarUrl() ?? target.GetDefaultAvatarUrl())
                    .WithText(target.Username);
            });

            return embed;
        }

        #region Message
        public static EmbedBuilder AddMessageContentField(EmbedBuilder embed, IMessage message, int cap = 200, string title = "Content", bool inline = false) {
            if (!string.IsNullOrWhiteSpace(message?.Content)) {
                if (message.Content.Length > cap) embed.AddField(title, $"{message.Content.Substring(0, cap)}...", inline);
                else embed.AddField(title, message.Content, inline);
            }
            return embed;
        }

        public static EmbedBuilder AddReactionsField(EmbedBuilder embed, IMessage message, int cap = 5, bool inline = false) {
            var reacts = new StringBuilder();
            int count = 0, sum = 0;
            foreach (var pair in message.Reactions.Take(cap)) {
                ++count; sum += pair.Value.ReactionCount;
                reacts.AppendLine($"  {pair.Key}: {pair.Value.ReactionCount}");
            }
            if (count > 0) embed.AddField($"Reactions (Count: {count}/{cap}, Sum: {sum})", reacts, inline);
            return embed;
        }

        public static EmbedBuilder AddAttachmentsField(EmbedBuilder embed, IMessage message, int cap = 5, bool inline = false) {
            string attachmentString = string.Join("\n", message.Attachments.Select(att => $"{att.Filename} - {att.Size / 1024.0:f2}KB ({att.Width}x{att.Height})"));
            if (!string.IsNullOrWhiteSpace(attachmentString)) embed.AddField($"Attachments", attachmentString, inline);

            return embed;
        }
        public static EmbedBuilder AddEmbedsField(EmbedBuilder embed, IMessage message, int cap = 5, bool inline = false) {
            string embedString = string.Join(", ", message.Embeds);
            if (embedString.Length > 0) embed.AddField($"Embeds", embedString, inline);

            return embed;
        }

        public static EmbedBuilder AddMessageDescription(EmbedBuilder embed, IMessage message) {
            var description = new StringBuilder().AppendLine($"**Message created**: {TextUtilities.Format(message.CreatedAt)}");
            if (message.EditedTimestamp.HasValue) description.AppendLine($"**Last edited**: {TextUtilities.Format(message.EditedTimestamp.Value)}");

            description
                .AppendLine($"**Message ID**: {message.Id}")
                .AppendLine(TextUtilities.MakeLink("Take me to the message", message.GetJumpUrl()));

            embed.WithDescription(description.ToString());

            return embed;
        }
        #endregion

        #region User
        public static EmbedBuilder AddUserClientField(EmbedBuilder embed, IUser user, bool inline = false) {
            if (user?.ActiveClients?.Count > 0) embed.AddField($"Clients", string.Join(", ", user.ActiveClients), inline);

            return embed;
        }
        public static EmbedBuilder AddUserActivityField(EmbedBuilder embed, IUser user, bool inline = false) {
            if (user?.Activities.FirstOrDefault() != null) {
                embed.AddField($"Activity", new StringBuilder()
                    .AppendLine($"{user.Activities.FirstOrDefault().Name}")
                    .AppendLine($"Type: {user.Activities.FirstOrDefault().Type}")
                    .AppendLine($"Details: {user.Activities.FirstOrDefault().Details}"),
                    inline);
            }

            return embed;
        }
        public static EmbedBuilder AddMutualGuildsField(EmbedBuilder embed, SocketUser user, bool inline = false) {
            if (user?.MutualGuilds?.Count > 1) embed.AddField($"MutualGuilds", string.Join(", ", user.MutualGuilds), inline);

            return embed;
        }
        public static EmbedBuilder AddUserDescription(EmbedBuilder embed, IUser user) {
            var sb = new StringBuilder()
                .AppendLine($"{user.Mention}")
                .AppendLine($"**Status**: {user.Status}")
                .AppendLine($"**Account created**: {TextUtilities.DateString(user.CreatedAt)}");

            if (user.IsBot) sb.AppendLine($"__User is a bot__");
            if (user.IsWebhook) sb.AppendLine($"__User is a webhook__");

            embed.WithDescription(sb.ToString());

            return embed;
        }
        #endregion

        #region Role
        public static EmbedBuilder AddRoleDescription(EmbedBuilder embed, SocketRole role) {
            var flags = new List<string>();
            if (role.IsEveryone) flags.Add("Everyone");
            if (role.IsManaged) flags.Add("Managed");
            if (role.IsHoisted) flags.Add("Hoisted");
            if (role.IsMentionable) flags.Add("Mentionable");

            var desc = new StringBuilder()
                .AppendLine(MentionUtils.MentionRole(role.Id))
                .AppendLine($"Name: {role.Name}")
                .AppendLine($"Position: {role.Position}")
                .AppendLine($"Color: {role.Color}")
                .AppendLine($"Creation date: {TextUtilities.Format(role.CreatedAt)}");

            var memberCount = role.Members.Count();
            if (memberCount > 0) desc.AppendLine($"Member Count: {memberCount}");
            if (flags.Count > 0) desc.AppendLine($"Flags: {string.Join(", ", flags)}");


            embed.WithDescription(desc.ToString());

            return embed;
        }

        public static EmbedBuilder AddPermissionsField(EmbedBuilder embed, SocketRole role) {
            var permStr = string.Join(", ", role.Permissions.ToList());

            if (permStr.Length > 0) embed.AddField($"Permissions", permStr);

            return embed;
        }
        #endregion

        #region AuditLogEntry
        public static EmbedBuilder AddAuditEntryField(EmbedBuilder embed, IAuditLogEntry entry) {
            if (entry != null) {
                embed.AddField("Done by", entry.User.Mention);
                if (!string.IsNullOrEmpty(entry.Reason)) embed.AddField("Reason", entry.Reason);

                if (embed.Footer == null) embed.WithFooter(entry.User.Username, entry.User.GetAvatarUrl() ?? entry.User.GetDefaultAvatarUrl());
                //if(embed.Author == null) embed.WithAuthor(entry.User);
            }

            return embed;
        }

        public static async Task<EmbedBuilder> AddAuditEntryField(EmbedBuilder embed, Task<RestAuditLogEntry> entryTask, string defaultReason = null) {
            if (entryTask == null) return embed;
            var entry = await entryTask;
            if (entry != null) {
                embed.AddField("Done by", entry.User.Mention);

                if (!string.IsNullOrEmpty(entry.Reason))
                    embed.AddField("Reason", entry.Reason);
                else if (!string.IsNullOrEmpty(defaultReason))
                    embed.AddField("Reason", defaultReason);
                //if(!string.IsNullOrEmpty(entry.Reason)) embed.AddField("Reason", entry.Reason);

                if (embed.Footer == null) embed.WithFooter(entry.User.Username, entry.User.GetAvatarUrl() ?? entry.User.GetDefaultAvatarUrl());
                //if(embed.Author == null) embed.WithAuthor(entry.User);
            }

            return embed;
        }
        #endregion
    }

    /*
		For commands
	*/
    public static partial class EmbedUtilities {

        /*
		=============================
			Full pages.
		=============================
		*/


        /*
			The page of a member of a guild.
		*/
        public static EmbedBuilder EmbedMemberInfoPage(
            this EmbedBuilder embed, GuildMemberSummary memberSummary, SocketGuildUser member, MemberNotes notes
        ) {
            embed
                .AddUserInfoField(memberSummary?.UserSummary, true)
                .AddGuildMemberInfoField(memberSummary, notes, true)
                .AddEmptyField(true)    // Space the fields out to keep them from getting squashed.
                .AddInviteField(memberSummary?.InviteUsed, true)
                //.EmbedGameInfo(memberSummary, notes)
                .AddRoleField(member)
                ;

            /// Get the highest role with a color

            if (member != null) {
                var role = member.Roles
                    .Where(role => role.Color != Color.Default)
                    .OrderByDescending(role => role.Position)
                    .FirstOrDefault()
                    ;
                if (role != null)
                    embed.WithColor(role.Color);
            }

            return embed
                .WithThumbnailUrl(memberSummary?.GetAvatar(size: 2048))
                .WithTitle($"Member Info")
                .SplitRows()
                ;
        }

        /*
			Page showing information about a player.
		*/
        public static EmbedBuilder EmbedPlayerStatusPage(
            this EmbedBuilder embed, IReadOnlyPlayer player, GuildMemberSummary memberSummary, SocketGuildUser member
        ) {
            var PlayerList = G.Services.GetService<PlayerList>();

            var leaders = PlayerList.Players.Leaderboard;
            var avg = leaders.Take(10).Average(plr => plr.Wallet);
            var prop = player.Wallet / Math.Max(1, avg);

            var gradient = GetPlayerGradient();

            return embed
                .EmbedPlayerInfo(player, memberSummary, inline: true)
                .EmbedPlayerRanks(player, memberSummary, inline: true)
                .WithColor(gradient.GetColor(Math.Clamp(prop, 0, 1)))
                .WithTitle($"{memberSummary?.Username}'s player")
                .WithThumbnailUrl(memberSummary?.GetAvatar(size: 2048))
                .SplitRows()
                ;
        }

        /*
			Page showing the stonks of the player.
		*/
        public static EmbedBuilder EmbedPlayerStonksPage(
            this EmbedBuilder embed, IReadOnlyPlayer player, GuildMemberSummary memberSummary
        ) {
            embed.AddPlayerStonksField(player);

            // Add a field with the stonks info
            return embed
                .WithTitle($"{memberSummary?.Username}'s stonks")
                //.WithThumbnailUrl(memberSummary?.GetAvatar(size: 2048))
                ;
        }

        /*
			Page showing the inventory of the player.
		*/
        public static EmbedBuilder EmbedInventoryPage(
            this EmbedBuilder embed, Inventory inventory, GuildMemberSummary memberSummary
        ) {
            double totalCount = 0, totalWeight = 0, totalValue = 0;

            foreach (var item in inventory.Items.Values) {
                embed.AddItemField(item);

                totalCount += item.Quantity;
                totalWeight += item.Weight;
                totalValue += item.Value;
            }

            /// Print cap info
            var sb = new StringBuilder()
                .AppendLine($"**Total Value:** {Player.CURRENCY_SYMBOL}{totalValue}")
                .AppendLine($"**Weight:** {Math.Round(totalWeight, 2)}/{Math.Round(inventory.WeightCap, 2)}")
                .AppendLine($"**Count:**  {totalCount:N0}/{inventory.ItemCap:N0}")
                ;

            embed.WithDescription(sb.ToString());

            //if(!inventory.EmbedInventory(embed))
            //	embed.AddField($"Inventory", "[Inventory is empty]");

            return embed
                .WithTitle($"{memberSummary?.Username}'s inventory")
                //.WithThumbnailUrl(memberSummary?.GetAvatar(size: 2048))
                ;
        }



        /*
			Page showing the user's profile.
		*/
        public static EmbedBuilder EmbedUserProfilePage(
            this EmbedBuilder embed, IReadOnlyPlayer player,
            GuildMemberSummary memberSummary, SocketGuildUser member,
            UserSummary userSummary, SocketUser user, MemberNotes notes
        ) {
            var PlayerList = G.Services.GetService<PlayerList>();

            var leaders = PlayerList.Players.Leaderboard;
            var avg = leaders.Take(10).Average(plr => plr.Wallet);
            var prop = (player?.Wallet ?? 0) / Math.Max(1, avg);

            var gradient = GetPlayerGradient();


            return embed
                .EmbedPlayerSummaryField(player, memberSummary, inline: true)
                .EmbedPlayerRanks(player, memberSummary, inline: true)

                .AddUserInfoField(userSummary, inline: true)
                .AddGuildMemberInfoField(memberSummary, notes, inline: true)

                .AddProfileOtherField(player, inline: true)

                .WithColor(gradient.GetColor(prop))
                .WithTitle($"{memberSummary?.Username}'s profile")
                .WithThumbnailUrl(memberSummary?.GetAvatar(size: 2048))

                .SplitRows()

                ;
        }


        /*
			Page showing the user's xp distribution.
		*/
        public static EmbedBuilder EmbedXpDistributionPage(this EmbedBuilder embed, IReadOnlyPlayer player) {
            return embed
                .AddXpDistributionField(player)
                .AddField("Target", player.Mention);
        }


        /*
			Page showing the user's penis.
		*/
        public static EmbedBuilder EmbedPenisPage(
            this EmbedBuilder embed, IUser target, Penis penis
        ) {
            var gradient = new ColorGradient(ColorCodes.Violet_Light.ToColor(), ColorCodes.Indigo.ToColor());

            embed
                .WithTitle("Penis Inspection")
                .WithColor(gradient.GetColor(penis.PenisScore))
                .WithDescription($@"**Target:** {target.Mention} 
				**Length:** {penis.Length}{penis.Unit}
				**Girth:** {penis.Girth}{penis.Unit}
				**Volume:** {penis.PenisVolume}{penis.Unit}^3 (Score: {penis.PenisVolumeScore:P0})
				**Ball Size:** {penis.BallSize}{penis.Unit} (Score: {penis.BallSizeScore:P0})
				**Cumshot Range:** {penis.CumRange}{penis.Unit} (Score: {penis.CumRangeScore:P0})
				**Score:** {penis.PenisScore:P0}
				{penis.PenisString}")
                .WithImageUrl(penis.PenisUrl)
                ;

            return embed;
        }


        /*
			Page showing the user's gayness.
		*/
        public static EmbedBuilder EmbedGaynessPage(
            this EmbedBuilder embed, IUser target, IReadOnlyPlayer player
        ) {

            var gradient = new ColorGradient(ColorCodes.Dodger_Blue.ToColor(), ColorCodes.Magenta_Light.ToColor());

            var gayness = new Gayness(player);

            //var res = RngCommands.GetGayness2(target);
            //var gayStr = RngCommands.GetGayString(res * 100);
            //var rating = Math.Clamp(res, 0, 1);

            embed
                .WithTitle("Gayness")
                .WithColor(gradient.GetColor(gayness.GaynessPercent))
                .WithDescription($"{target.Mention} is {gayness.GaynessValue:P} gay! \nThey {gayness.GaynessString}!")
                ;

            return embed;
        }


        /*
			Page showing info about the role.
		*/
        public static EmbedBuilder EmbedRoleInfoPage(this EmbedBuilder embed, RoleSummary summary) {
            embed
                .AddField("Role Info", new StringBuilder()
                    .AppendLine($"{summary.Mention}")
                    .AppendLine($"**Name**: {summary.Name}")
                    .AppendLine($"**Position**: {summary.Position}")
                    .AppendLine($"**Color**: #{summary.ColorValue:X}")
                    .AppendLine($"**Member Count**: {summary.MemberCount:N0}")
                    .AppendLine($"**Age**: {TextUtilities.Format(summary.CreatedAt.GetTimeSince())} ||({TextUtilities.Format(summary.CreatedAt, false)})||")
                    .ToString()
                ).AddFieldIf(summary.RawPermissions != GuildPermissions.None.RawValue, "Permissions",
                    string.Join(", ", summary.Permissions.ToList().Select(perm => $"`{perm}`"))
                ).WithColor(summary.Color)
                ;

            return embed;
        }

        /*
			Page showing info about the guild.
		*/
        public static EmbedBuilder EmbedGuildInfoPage(this EmbedBuilder embed, GuildSummary summary) {

            embed
                .AddField("General Info", new StringBuilder()
                    .AppendLine($"**Name**: {summary.Name}")
                    .AppendLine($"**Age**: {TextUtilities.Format(summary.CreatedAt.GetTimeSince())} ||({TextUtilities.Format(summary.CreatedAt, false)})||")
                    .AppendLine($"**Owner**: {MentionUtils.MentionUser(summary.OwnerId)}")
                    .ToString(), true
                ).AddEmptyField(true)
                .AddField("Game Info", new StringBuilder()
                    .AppendLine($"**Bot Subscription Level**: {summary.SubscriptionLevel}")
                    .AppendLine($"**Flags**: {summary.Flags}")
                    .AppendLine($"**Reserves**: {TextUtilities.Format(summary.Reserves)}")
                    .AppendLine($"**Income Tax**: {summary.IncomeTax:P0}")
                    .ToString(), true
                ).AddField("Stats", new StringBuilder()
                    .AppendLine($"**Members**: {summary.MemberCount}")
                    .AppendLine($"> **Bots**: {summary.BotCount}")
                    .AppendLine($"> **Users**: {summary.UserCount}")

                    .AppendLine($"**Channels**: {summary.ChannelCount}")
                    .AppendLine($"> **Text**: {summary.TextChannelCount}")
                    .AppendLine($"> **Voice**: {summary.VoiceChannelCount}")
                    .AppendLine($"> **Category**: {summary.CategoryChannelCount}")

                    .AppendLine($"**Roles**: {summary.RoleCount}")
                    .AppendLine($"")
                    .AppendLine($"**Booster Status**: {summary.PremiumTier} (Boosts: {summary.PremiumSubscriptionCount})")
                    .ToString(), true
                )
                .WithThumbnailUrl(summary.IconUrl)
                ;

            return embed;
        }




        /*
			Page showing the current modifiers of a player.
		*/
        public static EmbedBuilder EmbedModifiersPage(
            this EmbedBuilder embed, IReadOnlyPlayer player, IUserSummary userSummary
        ) {
            var modifiers = player.Modifiers;



            var sb = new StringBuilder();
            foreach (var (type, value) in modifiers.Modifiers) {
                var (min, max) = type.GetModifierRange();
                sb.AppendLine($"**{type}:** {value:P1} (Cap: [{min:P1}, {max:P1}])");
            }

            if (sb.Length == 0)
                sb.AppendLine($"User is currently not under any modifiers.");

            return embed
                .WithTitle($"{userSummary?.Username}'s modifiers")
                .WithDescription(new StringBuilder()
                    .AppendLine($"Showing a list of modifiers affecting the user.")
                    .AppendLine($"This might not include external modifiers, like xp bonus events or xp penalties.")
                    .AppendLine($"**Next Passive Expiration:** {TimestampTag.FromDateTime(modifiers.NextExpiration.UtcDateTime, TimestampTagStyles.Relative)}")
                    .ToString())
                .AddField("Modifiers", sb)
                //.WithThumbnailUrl(memberSummary?.GetAvatar(size: 2048)) 
                ;

        }

        /*
			Page showing the current modifiers of a player.
		*/
        public static EmbedBuilder EmbedPassiveEffectsPage(
            this EmbedBuilder embed, IReadOnlyPlayer player, IUserSummary userSummary
        ) {
            var modifiers = player.Modifiers;

            foreach (var passive in modifiers.PassiveEffects) {
                embed.AddPassiveEffectField(passive);
            }

            return embed
                .WithTitle($"{userSummary?.Username}'s passives")
                .WithDescription(new StringBuilder()
                    .AppendLine($"{player.Mention} has {modifiers.PassivesCount}/{UserModifiers.MAX_PASSIVES} passives.")
                    .ToString())
                //.WithThumbnailUrl(memberSummary?.GetAvatar(size: 2048))
                ;
        }


        /*
			Page showing the user's preferences.
		*/
        public static EmbedBuilder EmbedPreferencesPage(this EmbedBuilder embed, IUserPreference preference) {
            var dict = PreferenceCommands.PrintPreferences(preference).Result;

            return embed
                .WithTitle($"Preferences")
                .WithDescription(new StringBuilder()
                    .AppendJoin('\n', dict.Select(pair => $"**{pair.Key}:** {pair.Value}"))
                    .ToString()
                );

        }

        /*
		=============================
			Page components.
		=============================
		*/



        /*
			Shows tax info and note summary.
		*/
        public static EmbedBuilder EmbedGameInfo(
            this EmbedBuilder embed, GuildMemberSummary summary, MemberNotes notes = null, bool inline = false
        ) {
            var sb = new StringBuilder();
            var details = summary.MembershipDetails;

            sb
                //.AppendLineIf(details.AmountTaxed > 0, $"**Amount Taxed**: {TextUtils.Format(details.AmountTaxed)}")
                //.AppendLineIf(details.AmountContributed > 0, $"**Amount Contributed**: {TextUtils.Format(details.AmountContributed)}")
                .AppendLineIf(notes != null && notes.Count > 0, $"**Notes**: {notes?.Count}")
                ;

            if (sb.Length > 0)
                embed.AddField("Other Info", sb.ToString(), inline);

            return embed;
        }


        /*
			Adds info about the user.
		*/
        public static EmbedBuilder AddUserInfoField(
            this EmbedBuilder embed, IUserSummary summary,
            bool inline = false, bool addEmptyIfFalse = false
        ) {
            if (summary != null) {
                //StringBuilder sb = new StringBuilder();
                var sb = new StringBuilder()
                    .AppendLine($"{MentionUtils.MentionUser(summary.Id)}")
                    .AppendLine($"**Status**: {summary.Status}")
                    .AppendLine($"**Account Age**: {TextUtilities.Format(summary.CreatedAt.GetTimeSince())} ||({TextUtilities.GetTimestamp(summary.CreatedAt, format: 'd')})||");
                if (summary.IsBot) sb.AppendLine($"User is a bot");
                if (summary.IsWebhook) sb.AppendLine($"User is a webhook");
                //embed.WithDescription(sb.ToString());

                var userDetails = summary.UserDetails;
                if (userDetails != null) {
                    if (userDetails.CountTributesDone > 0)
                        sb.AppendLine($"**Tributed Done**: {userDetails.CountTributesDone}");

                    if (userDetails.CountTributesRecieved > 0)
                        sb.AppendLine($"**Tributed Recieved**: {userDetails.CountTributesRecieved}");
                }

                embed.AddFieldIf(sb.Length > 0, "User Info", sb.ToString(), inline: inline, addEmptyIfFalse: addEmptyIfFalse);
            } else if (addEmptyIfFalse) {
                embed.AddEmptyField(inline);
            }

            return embed;
        }


        /*
			Adds info about the member of the guild.
		*/
        public static EmbedBuilder AddGuildMemberInfoField(
            this EmbedBuilder embed, IGuildMemberSummary summary,
            MemberNotes notes = null,
            bool inline = false, bool addEmptyIfFalse = false
        ) {
            if (summary != null) {
                var details = summary.MembershipDetails;
                var sb = new StringBuilder();

                sb
                    .AppendLineIf(summary.TotalStayDuration > new TimeSpan(0, 5, 0),
                        $"**Member For**: {TextUtilities.Format(summary.TotalStayDuration)}")
                    .AppendLine($"**Latest Join**: {TextUtilities.GetTimestamp(summary.LatestJoin, format: 'd')}")

                    .AppendLineIf(summary.FirstJoin != summary.LatestJoin,
                        $"**First Join**: {TextUtilities.GetTimestamp(summary.FirstJoin, format: 'd')}")
                    .AppendLineIf(summary.FirstJoin != summary.LatestJoin,
                        $"**Join Count**: {summary.JoinCount}")
                    .AppendLineIf(details.LatestLeave >= details.LatestJoin,
                        $"**Left**: {TextUtilities.GetTimestamp(details.LatestLeave, format: 'd')}");


                /// VC status
                if (details.TotalVcTime > new TimeSpan(0, 5, 0)) {
                    // If user has spent enough time in vc, show how much of that was active
                    var validTimeStr = "";
                    if (details.TotalVcTime > new TimeSpan(24, 0, 0)) {
                        var validRate = Math.Round((details.ValidVcTime / details.TotalVcTime) / 5, 2) * 5;
                        validTimeStr = $" (~{validRate:P0} active)";
                    }
                    sb.AppendLine($"**Vc**: {TextUtilities.Format(details.TotalVcTime)}{validTimeStr}");
                }


                sb
                    .AppendLineIf(summary.MsgCount > 1,
                        $"**Messages**: {summary.MsgCount:N0}")
                    // Invite status
                    .AppendLineIf(details.CountInvited > 0,
                        $"**Invited**: {details.CountInvited} (New: {details.CountInvitedFirst})")
                    // Notes
                    .AppendLineIf(notes != null && notes.Count > 0,
                        $"**Notes**: {notes?.Count}")
                    ;

                embed.AddField("Member Info", sb.ToString(), inline);
            } else if (addEmptyIfFalse) {
                embed.AddEmptyField(inline);
            }

            return embed;
        }


        /*
			Adds info about the use of an invite.
		*/
        public static EmbedBuilder AddInviteField(this EmbedBuilder embed, InviteUseSummary invite, bool inline = false) {
            if (invite != null) {
                StringBuilder sb = new StringBuilder()
                    .AppendLine($"**Code**: {invite.Code}")
                    .AppendLine($"**Inviter**: {MentionUtils.MentionUser(invite.InviterId)}")
                    .AppendLine($"**Created**: {TextUtilities.Format(invite.CreatedAt, false)}")
                    .AppendLine($"**Use Count**: {invite.UseCount}");

                embed.AddField("Invite Used", sb.ToString(), inline);
            }

            return embed;
        }


        /*
			Adds info about the roles of a member.
		*/
        public static EmbedBuilder AddRoleField(this EmbedBuilder embed, SocketGuildUser user, bool inline = false) {
            if (user == null)
                return embed;

            var roles = user.Roles.Where(role => !role.IsEveryone);
            string roleString = string.Join(", ", roles.OrderByDescending(r => r.Position).Select(r => r.Mention));
            if (roleString.Length > 0) embed.AddField("Roles", roleString, inline);

            return embed;
        }



        /*
			Embeds the rankings of the player.
		*/
        public static EmbedBuilder EmbedPlayerRanks(
            this EmbedBuilder embed, IReadOnlyPlayer player, GuildMemberSummary member = null,
            bool inline = false, bool addEmptyIfFalse = false
        ) {
            var PlayerList = G.Services.GetService<PlayerList>();
            var OverviewService = G.Services.GetService<OverviewService>();

            var leaderRankings = PlayerList.GetLeaderBoardRanks(player.Id);

            // Append the rankings for the guild member if available.
            if (member != null) {
                var overview = OverviewService.GetOverview(member);

                foreach (var rankName in overview.ListNames) {
                    var rank = overview.GetRank(member.Id, rankName);
                    if (rank != int.MaxValue) leaderRankings[rankName] = rank;
                }
            }

            return embed.AddFieldIf(leaderRankings.Count > 0, $"Leaderboards",
                string.Join('\n', leaderRankings.Select(pair => $"**{pair.Key}**: {pair.Value + 1}")),
                inline: inline, addEmptyIfFalse: addEmptyIfFalse);
        }


        /*
			Embeds the info about a player.
		*/
        public static EmbedBuilder EmbedPlayerInfo(
            this EmbedBuilder embed, IReadOnlyPlayer player, GuildMemberSummary member, bool inline = false
        ) {
            //const int TITLE_WIDTH = 13;
            const int VALUE_WIDTH = 7;

            var LevelService = G.Services.GetService<LevelService>();

            // Task for getting the value of a user's stocks.
            //	! Done here because we have to wait for it later in the method !
            var stockValueTask = GetStonkValue(player);

            var available = player.AvailableWallet;
            var xpLeft = LevelService.GetLvlXpCost(player.Level) - player.XP;

            var playerXpRate = player.XpStatus.GetXpRate();
            var bonus = LevelService.GetBonuses(player);
            var xpMultiplier = playerXpRate * bonus;


            var sb = new StringBuilder();


            /*
				Add local levels.
			*/
            var prev = (double)LevelService.GetLvlXpCost(player.Level - 1);
            var next = (double)LevelService.GetLvlXpCost(player.Level);
            var rate = (player.XP - prev) / (next - prev);

            sb
                .AppendLine($"**Global Lvl:** {player.Level}")
                .AppendLine($"`{TextUtilities.ProgressBar(rate, 20)}`")
                ;

            //embed.AddField($"Global Level", new StringBuilder()
            //	.AppendLine($"{player.Level} (XP: {TextUtils.Format(player.XP)})")
            //	.AppendLine($"`{TextUtils.ProgressBar(rate, 20)}`")
            //, true);


            /*
				Add global levels.
			*/
            if (member != null) {
                var mPrev = (double)LevelService.GetLvlXpCost(member.Level - 1);
                var mNext = (double)LevelService.GetLvlXpCost(member.Level);
                var mRate = (member.XP - mPrev) / (mNext - mPrev);
                //embed.AddField($"Local Level", new StringBuilder()
                //	.AppendLine($"{member.Level} (XP: {TextUtils.Format(member.XP)})")
                //	.AppendLine($"`{TextUtils.ProgressBar(mRate, 20)}`")
                //, true);
                sb
                    .AppendLine($"**Local Lvl:** {member.Level}")
                    .AppendLine($"`{TextUtilities.ProgressBar(mRate, 20)}`")
                    ;
            }


            embed.AddField($"Levels", sb, true);

            /*
				Add the wallet info.
			*/

            // Get the value of the player's stonks.
            var stonkValue = stockValueTask.Result;
            var inventoryValue = player.Inventory.Value;
            var totalWallet = available + stonkValue + inventoryValue;

            var details = member?.MembershipDetails;


            embed
                .AddEmptyField(true)
                .AddField(Player.CURRENCY_NAME, new StringBuilder()
                    .AppendLine($"**Wallet:** {Player.CURRENCY_SYMBOL}{TextUtilities.Format(available)}")
                    .AppendLine($"**Stonks:** {Player.CURRENCY_SYMBOL}{TextUtilities.Format(stonkValue)}")
                    .AppendLine($"**Inventory:** {Player.CURRENCY_SYMBOL}{TextUtilities.Format(inventoryValue)}")
                    .AppendLine($"**Total:** {Player.CURRENCY_SYMBOL}{TextUtilities.Format(totalWallet)}")

                    .ToString(), true
                )
                .AddField("Other Info", new StringBuilder()
                    .AppendLine($"**Rank:** {player.PlayerRank?.Title}")
                    .AppendLine($"**Daily Streak:** {player.DailyStreak}")
                    .AppendLineIf(xpMultiplier != 1, $"**Xp Multiplier:** {xpMultiplier,VALUE_WIDTH:P0}")
                    .AppendLineIf(player.HasCovid, $"**Covid:** {TextUtilities.Format(player.TimeSincePositive)} ago.")
                    .AppendLineIf(details?.AmountTaxed > 0, $"**Taxed**: {Player.CURRENCY_SYMBOL}{TextUtilities.Format(details.AmountTaxed)}")
                    .AppendLineIf(details?.AmountContributed > 0, $"**Contributed**: {Player.CURRENCY_SYMBOL}{TextUtilities.Format(details.AmountContributed)}")
                    .AppendLine($"**Age:** {player.Age.TotalDays / 365:N0}")

                    .ToString(), true
                )
                .AddEmptyField(true)
                ;


            /*
				Add the other info.
			*/


            return embed;
        }



        /*
			Embeds simplified info about a player.
		*/
        public static EmbedBuilder EmbedPlayerSummaryField(
            this EmbedBuilder embed, IReadOnlyPlayer player, GuildMemberSummary member,
            bool inline = false, bool addEmptyIfFalse = false
        ) {
            if (player != null) {
                var available = player.AvailableWallet;
                var totalWallet = available + GetStonkValue(player).Result + player.Inventory.Value;


                embed.AddField($"Player Info", new StringBuilder()
                    .AppendLine($"**Global Lvl:** {player.Level}")
                    .AppendLineIf(member != null, $"**Local Lvl:** {member?.Level}")
                    .AppendLine($"**Rank:** {player.PlayerRank?.Title}")
                    .AppendLine($"**Wallet:** {Player.CURRENCY_SYMBOL}{TextUtilities.Format(available)}")
                    .AppendLine($"**Value:** {Player.CURRENCY_SYMBOL}{TextUtilities.Format(totalWallet)}")
                    .AppendLineIf(player.HasCovid, $"**Covid:** {TextUtilities.Format(player.TimeSincePositive)} ago.")
                    .AppendLine($"**Age:** {player.Age.TotalDays / 365:N0}")
                , inline: inline);
            } else if (addEmptyIfFalse) {
                embed.AddEmptyField(inline: inline);
            }

            return embed;
        }





        /*
			Adds a field with the player's stonks on it.
		*/
        public static EmbedBuilder AddPlayerStonksField(this EmbedBuilder embed, IReadOnlyPlayer player) {
            var StonkService = G.Services.GetService<StonkService>();


            var sb = new StringBuilder()
                // Add the table of the player's stonks.
                .Append(GetStonkTableString(player).Result)

                // Add the metainfo about the user's stonks.
                .Append("```")
                .AppendLine($"Company count:   {player.Stocks.Count:N0}/{StonkService.GetStockCompanyCap(player.PlayerRank):N0}")
                .AppendLine($"Cap pr company:  {StonkService.GetStockCap(player.PlayerRank):N}")
                .Append("```")
                ;



            // Add a field with the stonks info
            return embed
                .AddField($"Stonks", sb.ToString());
        }


        /*
			Adds a field with the player's xp distribution on it.
		*/
        public static EmbedBuilder AddXpDistributionField(this EmbedBuilder embed, IReadOnlyPlayer player) {

            var dist = player.XpStatus.XpStats;
            var runDur = DateTimeOffset.Now - player.XpStatus.TimeStamp;
            var total = Math.Max(1, dist.Values.Sum());


            var sb = new StringBuilder()
                .AppendLine("```")
                .AppendJoin('\n', dist.Select(pair => $"- {pair.Key,-10}: {pair.Value / runDur.TotalDays,9:N}/day, {(double)pair.Value / total,7:P1}"))
                .AppendLine()
                .AppendLine($"- {"TOTAL",-10}: {total / runDur.TotalDays,9:N}/day, {total / total,6:P}")
                .AppendLine($"RunTime: {TextUtilities.Format(runDur)}")
                .AppendLine("```");


            return embed
                .AddField("Xp Distribution", sb.ToString());

        }



        /*
			Adds info about a passive effect.
		*/
        public static EmbedBuilder AddPassiveEffectField(this EmbedBuilder embed, IPassiveEffect passive, bool inline = false) {
            var GameManager = G.Services.GetRequiredService<GameManager>();

            var sb = new StringBuilder()
                .AppendLine($"**Group**: {passive.GroupId:X}")
                .AppendLine($"**Effect**: `[{passive.ModifierType} | {passive.Value:P1}]`")
                .AppendLineIf(!string.IsNullOrWhiteSpace(passive.Description), $"**Description**: {passive.Description}")
                .AppendLine($"**Expires**: {TimestampTag.FromDateTime(passive.Expires.UtcDateTime, TimestampTagStyles.Relative)}")
                ;

            // Print all the SideEffects
            foreach (var sideEffect in passive.SideEffects) {
                var sideEffectType = GameManager.GetPassiveTypeOrDefault(sideEffect.Id);
                sb.AppendLine($"**SideEffect**: {sideEffectType}");
            }

            return embed.AddField(passive.Title, sb.ToString(), inline);
        }


        /*
			Adds info about a passive effect.
		*/
        public static EmbedBuilder AddItemField(this EmbedBuilder embed, Item item, bool inline = false) {
            var sb = new StringBuilder()
                .AppendLine($"**Properties:** `[{Player.CURRENCY_SYMBOL}{item.Value:N0} | {Math.Round(item.Weight, 2)}kg]`")
                .AppendLineIf(!string.IsNullOrWhiteSpace(item.Description), $"**Description:** {item.Description}")
                ;

            // If it's a consumable with an effect
            if (item is ConsumableItem consume) {
                var type = consume.Type;
                foreach (var passive in type.Effects)
                    sb.AppendLine($"**Effect:** {passive}");
            }

            return embed.AddField($"`[{item.Class}]` {item.Title} (x{Math.Round(item.Quantity, 2)})", sb.ToString(), inline);
        }


        /*
			Adds info about a passive effect.
		*/
        public static EmbedBuilder AddPlayerStatusField(this EmbedBuilder embed, IReadOnlyPlayer player, bool inline = false) {
            var modifiers = player.Modifiers;

            var sb = new StringBuilder()
                .AppendLineIf(modifiers.PassivesCount > 0, $"**Passives:** {modifiers.PassivesCount}")
                .AppendLineIf(modifiers.ModifierCount > 0, $"**Modifiers:** {modifiers.ModifierCount}")
                ;

            if (sb.Length > 0)
                embed.AddField("Status Effects", sb.ToString(), inline);

            return embed;
        }


        /*
			Adds other info for profile page.
		*/
        public static EmbedBuilder AddProfileOtherField(
            this EmbedBuilder embed, IReadOnlyPlayer player,
            bool inline = false, bool addEmptyIfFalse = false
        ) {
            var modifiers = player.Modifiers;

            var family = player.Family;
            var relativesCount = family.Parents.Count + family.Children.Count;

            var sb = new StringBuilder()
                .AppendLineIf(modifiers.PassivesCount > 0, $"**Passives:** {modifiers.PassivesCount}")
                .AppendLineIf(modifiers.ModifierCount > 0, $"**Modifiers:** {modifiers.ModifierCount}")
                .AppendLineIf(family.Partner.HasValue, $"**Partner:** {MentionUtils.MentionUser(family.Partner ?? 0)}")
                .AppendLineIf(relativesCount > 0, $"**Relatives:** {relativesCount}")
                ;

            return embed.AddFieldIf(sb.Length > 0, "Other Info", sb.ToString(), inline: inline, addEmptyIfFalse: addEmptyIfFalse);
        }






        /*
		=============================
			Needed methods.
		=============================
		*/


        /*
			Gets the values of a user's stonks.
		*/
        public static async Task<double> GetStonkValue(IReadOnlyPlayer player) {
            var StonkService = G.Services.GetService<StonkService>();

            // Get the sum
            double sum = 0;
            foreach (var pair in player.Stocks) {
                var stonk = await StonkService.GetStonk(pair.Key);
                var sc = pair.Value;

                sum += stonk.BotPrice * sc.Count;
            }

            return sum;
        }



        /*
			Gets the values of a user's stonks.
		*/
        public static async Task<string> GetStonkTableString(IReadOnlyPlayer player) {
            const int TICKER_LENGTH = 8,
                NUMBER_LENGTH = 8,
                COUNT_LENGTH = 8;

            var StonkService = G.Services.GetService<StonkService>();

            var stonks = player.Stocks;

            // If the player doesn't have any stonks.
            if (stonks.Count == 0)
                return $"``` Player has no stonks. ```";


            // Builder for stonk table
            var table = new TableBuilder(-TICKER_LENGTH, COUNT_LENGTH, NUMBER_LENGTH, NUMBER_LENGTH, NUMBER_LENGTH) {
                RowGroupSize = 5,
                LongColumnValueHandling = LongColumnValueHandling.Shorten,
                //RowSurounding = RowSuroundingOptions.None,
                ValueMargin = false,
            }.WithHeader("Ticker", "Count", "Value", "Cost", "Diff");


            // Go through all the user's stonks.
            double sum = 0, count = 0, sumCost = 0;
            foreach (var pair in stonks) {

                // Get the stonk.
                var stonk = await StonkService.GetStonk(pair.Key);
                var sc = pair.Value;

                // Get the current value of the collection and the difference.
                double value = stonk.BotPrice * sc.Count,
                    diff = value - sc.Cost;

                // Update the totals.
                count += sc.Count; sum += value; sumCost += sc.Cost;

                // Add a row with the stonk's details.
                table.AddRow(pair.Key, TextUtilities.Format(sc.Count), TextUtilities.Format(value), TextUtilities.Format(sc.Cost), TextUtilities.Format(diff));
            }

            // Add the row with the total values of the stonks.
            table.AddRow("TOTAL", TextUtilities.Format(count), TextUtilities.Format(sum), TextUtilities.Format(sumCost), TextUtilities.Format(sum - sumCost));

            // Build the table and return the string.
            return table.Build();
        }
    }



    /*
		Extensions
	*/
    public static partial class EmbedUtilities {

        public static EmbedBuilder AddEmptyField(this EmbedBuilder embed, bool inline = false) {
            return embed.AddField("\u200b", "\u200b", inline);
        }
        public static EmbedBuilder AddFieldIf(this EmbedBuilder embed, bool condition, string name, object value, bool inline = false, bool addEmptyIfFalse = false) {
            if (condition) embed.AddField(name, value, inline);
            else if (addEmptyIfFalse) embed.AddEmptyField(inline);
            return embed;
        }


        /// <summary>
        ///     Enforces that there are no more than 2 fields in any row.
        /// </summary>
        /// <param name="embedBuilder"></param>
        /// <param name="removeEmptyFields"></param>
        /// <param name="forceLastLineGrid"></param>
        /// <param name="sortByHeight"></param>
        /// <returns></returns>
        public static EmbedBuilder SplitRows(
            this EmbedBuilder embedBuilder, bool removeEmptyFields = true, bool forceLastLineGrid = true, bool sortByHeight = true
        ) {
            var emptyField = new EmbedFieldBuilder()
                .WithIsInline(true)
                .WithName($"_ _")
                .WithValue($"_ _");

            bool IsEmpty(EmbedFieldBuilder field)
                => field.Name == "\u200b" && field.Value.ToString() == "\u200b";

            IEnumerable<EmbedFieldBuilder> fields = embedBuilder.Fields;

            //Remove empty fields.
            if (removeEmptyFields) fields = fields.Where(field => !IsEmpty(field));

            //Sort by the number of linebreaks in the field value.
            if (sortByHeight) fields = fields.OrderByDescending(field => field.Value.ToString().Count(chr => chr == '\n'));

            var res = new List<EmbedFieldBuilder>();
            int fieldCount = 0;

            //Go through each field in the embed.
            foreach (var field in fields) {
                //If the field is inline
                if (field.IsInline) {
                    //If we already have 2 fields on this row.
                    if (fieldCount == 2) {
                        //Add an empty field to force it on the next line.
                        res.Add(emptyField);

                        //Since we're now on the next line: Reset the count.
                        fieldCount = 0;
                    }

                    //Add the field.
                    res.Add(field);
                    ++fieldCount;
                }

                //If the field isn't inline.
                else {
                    //Add the field and reset the counter.
                    res.Add(field);
                    fieldCount = 0;
                }
            }

            //Add an empty field at the end if it has 2 fields in the last row.
            //This forces it to conform to the same width as the other lines.
            if (forceLastLineGrid && fieldCount == 2) res.Add(emptyField);

            //Replace the fields of this embed.
            embedBuilder.Fields = res;

            return embedBuilder;
        }
    }
}