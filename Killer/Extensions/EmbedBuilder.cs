using Discord;
using KillersLibrary.Enums;

namespace KillersLibrary.Extensions {
    public static class EmbedBuilderExtension {
        public static EmbedBuilder WithColorCode(this EmbedBuilder embedBuilder, ColorCodes colorCodes) {
            embedBuilder.WithColor(new Color((uint)colorCodes));
            return embedBuilder;
        }

        public static EmbedBuilder WithHexColor(this EmbedBuilder embedBuilder, string hexCode) {
            bool success = uint.TryParse("0x" + hexCode.Replace("#", ""), out uint hex);

            embedBuilder.WithColor(new Color(success ? hex : 0x000000));
            return embedBuilder;
        }
    }
}
