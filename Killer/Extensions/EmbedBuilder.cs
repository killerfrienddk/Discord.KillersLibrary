using Discord;
using KillersLibrary.Enums;

namespace KillersLibrary.Extensions {
    public static class EmbedBuilderExtension {
        public static EmbedBuilder WithColorCode(this EmbedBuilder embedBuilder, ColorCodes colorCodes) {
            embedBuilder.WithColor(new Color((uint)colorCodes));
            return embedBuilder;
        }
        }
    }
}
