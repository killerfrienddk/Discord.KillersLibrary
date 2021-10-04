using Discord;

namespace Interaction.TestingGrounds.Utilities {
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
    }

    /*
		For commands
	*/
    public static partial class EmbedUtilities {

    }
}