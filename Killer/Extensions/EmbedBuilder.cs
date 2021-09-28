using Discord;
using KillersLibrary.Enums;

namespace KillersLibrary.Extensions {
    public static class EmbedBuilderExtension {
        /// <summary> Adds embed color based on the provided <see cref="ColorCodes"/>.</summary>
        /// <param name="colorCodes">Choose which color you want using <see cref="ColorCodes"/></param>
        public static EmbedBuilder WithColorCode(this EmbedBuilder embedBuilder, ColorCodes colorCodes = ColorCodes.Default) {
            embedBuilder.WithColor(new Color((uint)colorCodes));

            return embedBuilder;
        }

        /// <summary> Adds embed color based on the provided hex code e.g #000000.</summary>
        /// <param name="hexCode">Choose which color using a hex code.</param>
        public static EmbedBuilder WithHexColor(this EmbedBuilder embedBuilder, string hexCode) {
            bool success = uint.TryParse("0x" + hexCode.Replace("#", ""), out uint hex);

            embedBuilder.WithColor(new Color(success ? hex : 0x000000));

            return embedBuilder;
        }
            return embedBuilder;
        }
    }
}
