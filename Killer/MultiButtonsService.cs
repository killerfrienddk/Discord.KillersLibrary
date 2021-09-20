using System.Collections.Generic;
using System.Linq;
using System;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;

namespace KillersLibrary {
    public class MultiButton {
        public string Title { get; set; }
        public string Value { get; set; }
    }


    public class BaseStyle {
        public string CustomID { get; set; }
        public ButtonStyle ButtonStyle { get; set; } = ButtonStyle.Success;
        public bool OrderByTitle { get; set; } = true;
    }

    public class MultiButtonsStyles : BaseStyle {
        public bool UpperCaseLetters { get; set; } = true;
    }

    public class SelectForMultiButtonsStyles : BaseStyle {
        public string Placeholder { get; set; } = "Select Item";
        public bool RagedLettersOnEndOfPlaceholder { get; set; } = true;
    }

    public class MultiButtonsService {
        /// <summary>
        ///     Creates Multi Buttons.
        /// </summary>
        /// <param name="titles">List of strings that will be placed on the buttons title e.g A-B"/></param>
        /// <param name="styles">The <see cref="MultiButtonsStyles"/> is for customization of many parameters.</param>
        /// <returns>A <see cref="ComponentBuilder"/></returns>
        public virtual ComponentBuilder CreateMultiButtons(List<string> titles, MultiButtonsStyles styles = null) {
            styles ??= new();

            if (styles.OrderByTitle) titles = titles.OrderBy(t => t).ToList();

            var builder = new ComponentBuilder();
            int count = 1;
            int resetAt = 0;
            int buttonCount = 0;
            string firstLetter = "";
            string lastLetter;
            for (int i = 0; i < titles.Count; i++) {
                if (count == 1) firstLetter = titles[i][0].ToString();

                if (count == 25) {
                    lastLetter = titles[i][0].ToString();
                    count = 1;
                    resetAt = i + 1;
                    ButtonBuilder button = new();
                    string letters = "";
                    if (string.IsNullOrWhiteSpace(firstLetter)) {
                        letters = styles.UpperCaseLetters ? lastLetter.ToUpper() : lastLetter.ToLower();
                    } else {
                        letters = (styles.UpperCaseLetters ? firstLetter.ToUpper() : firstLetter.ToLower()) +
                        "-" + (styles.UpperCaseLetters ? lastLetter.ToUpper() : lastLetter.ToLower());
                    }

                    button.WithLabel(letters);
                    button.WithStyle(styles.ButtonStyle);
                    button.WithCustomId(styles.CustomID + ++buttonCount);

                    builder.WithButton(button);

                    firstLetter = "";
                } else count++;
            }

            if (count >= 2) {
                if (resetAt > titles.Count) resetAt++;
                firstLetter = titles[resetAt][0].ToString();
                lastLetter = titles[resetAt + count - 2][0].ToString();

                ButtonBuilder button = new();
                string letters = "";
                if (string.IsNullOrWhiteSpace(firstLetter)) {
                    letters = styles.UpperCaseLetters ? lastLetter.ToUpper() : lastLetter.ToLower();
                } else {
                    letters = (styles.UpperCaseLetters ? firstLetter.ToUpper() : firstLetter.ToLower()) +
                    "-" + (styles.UpperCaseLetters ? lastLetter.ToUpper() : lastLetter.ToLower());
                }
                button.WithLabel(letters);
                button.WithStyle(styles.ButtonStyle);
                button.WithCustomId(styles.CustomID + ++buttonCount);

                builder.WithButton(button);
            }

            return builder;
        }


        /// <summary>
        ///     This is for choosing the amount of rows pr message.
        /// </summary>
        public enum MultiButtonsRows {
            One = 125,
            Two = 250,
            Three = 375,
            Four = 500,
            Five = 650
        }

        /// <summary>
        ///     Creates Multiple Multi Buttons.
        /// </summary>
        /// <param name="titles">List of strings that will be placed on the buttons title e.g A-B"/></param>
        /// <param name="styles">The <see cref="MultiButtonsStyles"/> is for customization of many parameters.</param>
        /// <returns>A list of <see cref="ComponentBuilder"/></returns>
        public virtual List<ComponentBuilder> CreateMultipleMultiButtons(List<string> titles, MultiButtonsRows multiButtonsRows = MultiButtonsRows.Five, MultiButtonsStyles styles = null) {
            List<ComponentBuilder> componentBuilders = new();

            for (int i = 0; i < titles.Count; i++) {
                var strings = titles.Skip(i + (int)multiButtonsRows).Take((int)multiButtonsRows).ToList();
                componentBuilders.Add(CreateMultiButtons(strings, styles));
            }

            return componentBuilders;
        }

        /// <summary>
        ///     Creates Select For Multi Buttons.
        /// </summary>
        /// <param name="interaction">The <see cref="SocketMessageComponent"/> is used for getting the number at the end of the customID. <see cref="SocketMessageComponent"/></param>
        /// <param name="multiButtons">List of <see cref="MultiButton"/>s that will be calculated placed in the select depending on the range that has been choosen."/></param>
        /// <param name="styles">The <see cref="SelectForMultiButtonsStyles"/> is for customization of many parameters.</param>
        /// <returns>A <see cref="ComponentBuilder"/></returns>
        [Obsolete("This method will soon be deprecated and will be removed in future versions. Please use the new version instead CreateSelectForMultiButtonsAsync.", true)]
        public virtual ComponentBuilder CreateSelectForMultiButtons(SocketMessageComponent interaction, List<MultiButton> multiButtons, SelectForMultiButtonsStyles styles = null) {
            styles ??= new();

            int number = Convert.ToInt32(string.Join("", interaction.Data.CustomId.Where(c => char.IsDigit(c))));

            SelectMenuBuilder selectMenu = new SelectMenuBuilder()
                .WithCustomId(styles.CustomID);

            string lastLetter = "";
            if (styles.OrderByTitle) multiButtons = multiButtons.OrderBy(m => m.Title).ToList();
            try {
                for (int i = (number * 25) - 25; i < number * 25; i++) {
                    selectMenu.AddOption(multiButtons[i].Title, multiButtons[i].Value);
                    lastLetter = multiButtons[i].Title[0].ToString();
                }
            } catch (Exception) { }

            string rangeLetters = $"{multiButtons[(number * 25) - 25].Title[0]}-{lastLetter}";

            selectMenu.WithPlaceholder(styles.Placeholder +
                (styles.RagedLettersOnEndOfPlaceholder ? " - " + rangeLetters : ""));

            return new ComponentBuilder().WithSelectMenu(selectMenu);
        }

        /// <summary>
        ///     Creates Select For Multi Buttons.
        /// </summary>
        /// <param name="interaction">The <see cref="SocketMessageComponent"/> is used for getting the number at the end of the customID. <see cref="SocketMessageComponent"/></param>
        /// <param name="multiButtons">List of <see cref="MultiButton"/>s that will be calculated placed in the select depending on the range that has been choosen."/></param>
        /// <param name="styles">The <see cref="SelectForMultiButtonsStyles"/> is for customization of many parameters.</param>
        /// <returns>A <see cref="ComponentBuilder"/></returns>
        public virtual async Task<ComponentBuilder> CreateSelectForMultiButtonsAsync(SocketMessageComponent interaction, List<MultiButton> multiButtons, SelectForMultiButtonsStyles styles = null) {
            await interaction.DeferAsync();
            styles ??= new();

            int number = Convert.ToInt32(string.Join("", interaction.Data.CustomId.Where(c => char.IsDigit(c))));

            SelectMenuBuilder selectMenu = new SelectMenuBuilder()
                .WithCustomId(styles.CustomID);

            string lastLetter = "";
            if (styles.OrderByTitle) multiButtons = multiButtons.OrderBy(m => m.Title).ToList();
            try {
                for (int i = (number * 25) - 25; i < number * 25; i++) {
                    selectMenu.AddOption(multiButtons[i].Title, multiButtons[i].Value);
                    lastLetter = multiButtons[i].Title[0].ToString();
                }
            } catch (Exception) { }

            string rangeLetters = $"{multiButtons[(number * 25) - 25].Title[0]}-{lastLetter}";

            selectMenu.WithPlaceholder(styles.Placeholder +
                (styles.RagedLettersOnEndOfPlaceholder ? " - " + rangeLetters : ""));

            return new ComponentBuilder().WithSelectMenu(selectMenu);
        }
    }
}
