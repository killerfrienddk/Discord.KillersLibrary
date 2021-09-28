﻿using System.Collections.Generic;
using System.Linq;
using Discord;
using KillersLibrary.Enums;

namespace KillersLibrary.Extensions {
    public static class EmbedBuilderExtension {
        #region Colors EmbedBuilder Extensions
        /// <summary> Adds embed color based on the provided <see cref="ColorCodes"/>.</summary>
        /// <param name="colorCodes">Choose which color you want using <see cref="ColorCodes"/></param>,
        /// <returns>The current builder.</returns>
        public static EmbedBuilder WithColorCode(this EmbedBuilder embedBuilder, ColorCodes colorCodes = ColorCodes.Default) {
            embedBuilder.WithColor(new Color((uint)colorCodes));

            return embedBuilder;
        }

        /// <summary> Adds embed color based on the provided hex code, e.g #000000.</summary>
        /// <param name="hexCode">Choose which color using a hex code.</param>
        /// <returns>The current builder.</returns>
        public static EmbedBuilder WithHexColor(this EmbedBuilder embedBuilder, string hexCode) {
            bool success = uint.TryParse("0x" + hexCode.Replace("#", ""), out uint hex);

            embedBuilder.WithColor(new Color(success ? hex : 0x000000));

            return embedBuilder;
        }
        #endregion

        //Got this from True Love he allowed me to copy it and modified it abit.

        #region Structuring EmbedBuilder Extensions
        //Got this from True Love he allowed me to copy it and modified it a bit.
        /// <summary>Sorts columns in to rows, two side by side.</summary>
        /// <param name="removeEmptyFields">Wheather or not you want to remove empty fields.</param>
        /// <param name="forceLastLineGrid">Wheather or not you want to enforce the last row to be two columns.</param>
        /// <param name="sortByHeight">Wheather or not you want it to sort by height.</param>
        /// <returns>The current builder.</returns>
        public static EmbedBuilder ColumnCombiner(
            this EmbedBuilder embedBuilder, bool removeEmptyFields = true, bool forceLastLineGrid = true, bool sortByHeight = true
        ) {
            IEnumerable<EmbedFieldBuilder> fields = embedBuilder.Fields;

            //Remove empty fields.
            if (removeEmptyFields) fields = fields.Where(field => !IsEmpty(field));

            //Sort by the number of linebreaks in the field value.
            if (sortByHeight) fields = fields.OrderByDescending(field => field.Value.ToString().Count(chr => chr == '\n'));

            int fieldCount = 0;
            List<EmbedFieldBuilder> res = new();

            //Creates an empty field.
            EmbedFieldBuilder emptyField = new EmbedFieldBuilder()
                .WithIsInline(true)
                .WithName($"_ _")
                .WithValue($"_ _");

            //Go through each field in the embed.
            foreach (var field in fields) {
                if (field.IsInline) { //If the field is inline
                    //If we already have 2 fields on this row.
                    if (fieldCount == 2) {
                        //Add an empty field to force it on the next line.
                        res.Add(emptyField);

                        //Since we're now on the next line: Reset the count.
                        fieldCount = 0;
                    }

                    //Add the field.
                    res.Add(field);
                    fieldCount++;
                } else { //If the field isn't inline.
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
        #endregion

        #region Add Field EmbedBuilder Extensions
        //Got this from True Love he allowed me to copy it and modified it a bit.
        /// <summary>
        ///     Adds an Discord.Embed field with the provided name and value.
        /// </summary>
        /// <param name="embedBuilder"></param>
        /// <param name="inline">Indicates whether the field is in-line or not.</param>
        /// <returns>The current builder.</returns>
        public static EmbedBuilder AddEmptyField(this EmbedBuilder embedBuilder, bool inline = false) {
            return embedBuilder.AddField("\u200b", "\u200b", inline);
        }
        #endregion

        #region Helper EmbedBuilder Extensions
        //Got this from True Love he allowed me to copy it and modified it a bit.
        private static bool IsEmpty(EmbedFieldBuilder field) => field.Name == "\u200b" && field.Value.ToString() == "\u200b";
        #endregion
    }
}
