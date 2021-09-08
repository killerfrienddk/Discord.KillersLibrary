using System.Collections.Generic;
using System.Linq;
using System;
using Discord.WebSocket;
using Discord;

namespace KillersLibrary {
    public class MultiButton {
        public string Title { get; set; }
        public string Value { get; set; }
    }

    public class MultiButtonsStyles {
        public string CustomID { get; set; } = "multiButtons";
        public ButtonStyle ButtonStyle { get; set; } = ButtonStyle.Success;
        public bool UpperCaseLetters { get; set; } = true;
        public bool OrderByTitle { get; set; } = true;
    }

    public class SelectForMultiButtonsStyles {
        public string CustomID { get; set; } = "chooseRange";
        public string Placeholder { get; set; } = "Select Item";
        public bool RagedLettersOnEndOfPlaceholder { get; set; } = true;
        public bool OrderByTitle { get; set; } = true;
    }

    public class MultiButtonsService {
        /// <summary>
        ///     Creates Multi Buttons.
        /// </summary>
        /// <param name="titles">List of strings that will be placed on the buttons title e.g A-B"/></param>
        /// <param name="styles">Styling or customization of the buttons. <see cref="MultiButtonsStyles"/></param>
        public ComponentBuilder CreateMultiButtons(List<string> titles, MultiButtonsStyles styles = null) {
            styles ??= new();

            var builder = new ComponentBuilder();

            int count = 1;
            string firstLetter = "";
            int resetAt = 0;
            int buttonCount = 0;

            string lastLetter;
            if (styles.OrderByTitle) titles = titles.OrderBy(t => t).ToList();

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
        ///     Creates Select For Multi Buttons.
        /// </summary>
        /// <param name="interaction">Interaction. <see cref="SocketMessageComponent"/></param>
        /// <param name="multiButtons">List of multiButtons that will be calculated placed in the select depending on the range that has been choosen."/></param>
        /// <param name="styles">Styling or customization of the buttons. <see cref="SelectForMultiButtonsStyles"/></param>
        public ComponentBuilder CreateSelectForMultiButtons(SocketMessageComponent interaction, List<MultiButton> multiButtons, SelectForMultiButtonsStyles styles = null) {
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

            var builder = new ComponentBuilder().WithSelectMenu(selectMenu);

            return builder;
        }
    }
}
