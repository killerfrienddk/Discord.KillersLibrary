using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Discord.WebSocket;
using Discord;
using KillersLibrary.Utilities;

namespace KillersLibrary.Services {
    /// <summary>
    ///     This is for choosing the amount of rows pr message.
    /// </summary>
    public enum MultiButtonsRows {
        One = 125,
        Two = 250,
        Three = 375,
        Four = 500,
        Five = 625
    }

    public class MultiButton {
        public string Title { get; set; }
        public string Value { get; set; }
        public bool Default { get; set; }
        public Optional<string> Description { get; set; }
        public Optional<IEmote> Emote { get; set; }
    }

    public class BaseStyle {
        public string CustomID { get; set; }
        public ButtonStyle ButtonStyle { get; set; } = ButtonStyle.Success;
        public bool OrderByTitle { get; set; } = true;
    }

    public class MultiButtonsStyles : BaseStyle {
        public bool UpperCaseLetters { get; set; } = true;
    }

    public class MultipleMultiButtonsStyles : MultiButtonsStyles {
        public bool DoNotAddLastButton { get; set; } = false;
        public MultiButtonsRows MultiButtonsRows = MultiButtonsRows.Five;
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
            Preconditions.AtMost(titles.Count, (int)MultiButtonsRows.Five, "Buttons", $"If you have more than {(int)MultiButtonsRows.Five} then please use the {nameof(CreateMultipleMultiButtons)} function.");
            return CreateMultiButtonsInternal(titles, styles).Item1;
        }

        private (ComponentBuilder, int buttonCount) CreateMultiButtonsInternal(List<string> titles, MultiButtonsStyles styles = null, int buttonCount = 0) {
            styles ??= new();

            if (styles.OrderByTitle) titles = titles.OrderBy(t => t).ToList();

            var builder = new ComponentBuilder();
            int count = 1;
            int resetAt = 0;
            string firstLetter = "";
            string lastLetter;
            for (int i = 0; i < titles.Count; i++) {
                if (count == 1) firstLetter = titles[i][0].ToString();

                if (count == 25) {
                    count = 1;

                    lastLetter = titles[i][0].ToString();
                    lastLetter = styles.UpperCaseLetters ? lastLetter.ToUpper() : lastLetter.ToLower();
                    firstLetter = styles.UpperCaseLetters ? firstLetter.ToUpper() : firstLetter.ToLower();

                    string letters = "";
                    if (string.IsNullOrWhiteSpace(firstLetter) || firstLetter == lastLetter) letters = firstLetter;
                    else letters = $"{firstLetter}-{lastLetter}";

                    ButtonBuilder button = new();
                    button.WithLabel(letters);
                    button.WithStyle(styles.ButtonStyle);
                    button.WithCustomId(styles.CustomID + ++buttonCount);

                    builder.WithButton(button);

                    firstLetter = "";
                    resetAt = i + 1;
                } else count++;
            }

            if (count >= 2) {
                if (resetAt > titles.Count) resetAt++;

                lastLetter = titles[resetAt + count - 2][0].ToString();
                lastLetter = styles.UpperCaseLetters ? lastLetter.ToUpper() : lastLetter.ToLower();

                firstLetter = titles[resetAt][0].ToString();
                firstLetter = styles.UpperCaseLetters ? firstLetter.ToUpper() : firstLetter.ToLower();

                string letters = "";
                if (string.IsNullOrWhiteSpace(firstLetter) || firstLetter == lastLetter) letters = firstLetter;
                else letters = $"{firstLetter}-{lastLetter}";

                ButtonBuilder button = new();
                button.WithLabel(letters);
                button.WithStyle(styles.ButtonStyle);
                button.WithCustomId(styles.CustomID + ++buttonCount);

                builder.WithButton(button);
            }

            return (builder, buttonCount);
        }

        /// <summary>
        ///     Creates Multiple Multi Buttons.
        /// </summary>
        /// <param name="titles">List of strings that will be placed on the buttons title e.g A-B"/></param>
        /// <param name="styles">The <see cref="MultiButtonsStyles"/> is for customization of many parameters.</param>
        /// <returns>A list of <see cref="ComponentBuilder"/></returns>
        public virtual List<ComponentBuilder> CreateMultipleMultiButtons(List<string> titles, MultipleMultiButtonsStyles styles = null) {
            styles ??= new();

            if (styles.OrderByTitle) {
                titles = titles.OrderBy(t => t).ToList();
                styles.OrderByTitle = false; // Prevets the CreateMultiButtonsInternal from doing it Ordering aswell.
            }

            List<ComponentBuilder> componentBuilders = new();
            int buttonCount = 0;
            for (int i = 0; i * (int)styles.MultiButtonsRows < titles.Count; i++) {
                var strings = titles.Skip(i * (int)styles.MultiButtonsRows - (styles.DoNotAddLastButton ? 25 : 0)).Take((int)styles.MultiButtonsRows - (styles.DoNotAddLastButton ? 25 : 0)).ToList();
                (ComponentBuilder, int) multiButton = CreateMultiButtonsInternal(strings, styles, buttonCount);
                componentBuilders.Add(multiButton.Item1);
                buttonCount = multiButton.Item2;
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
        public virtual ComponentBuilder CreateSelectForMultiButtons(SocketMessageComponent interaction, List<MultiButton> multiButtons, SelectForMultiButtonsStyles styles = null) {
            styles ??= new();

            int number = Convert.ToInt32(string.Join("", interaction.Data.CustomId.Where(c => char.IsDigit(c))));

            SelectMenuBuilder selectMenu = new SelectMenuBuilder()
                .WithCustomId(styles.CustomID);

            string lastLetter = "";
            if (styles.OrderByTitle) multiButtons = multiButtons.OrderBy(m => m.Title).ToList();
            try {
                for (int i = (number * 25) - 25; i < number * 25; i++) {
                    SelectMenuOptionBuilder option = new();
                    option.WithDefault(multiButtons[i].Default);
                    option.WithLabel(StringLengthFixer(multiButtons[i].Title, 100));
                    option.WithValue(StringLengthFixer(multiButtons[i].Value, 100));
                    if (multiButtons[i].Description.IsSpecified) option.WithDescription(StringLengthFixer(multiButtons[i].Description.Value, 100));
                    if (multiButtons[i].Emote.IsSpecified) option.WithEmote(multiButtons[i].Emote.Value);

                    selectMenu.AddOption(option);
                    lastLetter = multiButtons[i].Title[0].ToString();
                }
            } catch (Exception) { }

            string rangeLetters = "";
            string firstLetter = multiButtons[(number * 25) - 25].Title[0].ToString();
            if (firstLetter == lastLetter) rangeLetters = lastLetter;
            else rangeLetters = $"{firstLetter}-{lastLetter}";

            selectMenu.WithPlaceholder(StringLengthFixer(styles.Placeholder +
                        (styles.RagedLettersOnEndOfPlaceholder ? " - " + rangeLetters : ""), 100));

            return new ComponentBuilder().WithSelectMenu(selectMenu);
        }

        private string StringLengthFixer(string text, int maxLength) {
            return text.Substring(0, text.Length > maxLength ? maxLength : text.Length);
        }

        public async Task<IMessage> RemoveMultiButtonsAndSelectAsync(SocketMessageComponent interaction) {
            IMessage repliedIMessage = await interaction.Channel.GetMessageAsync(interaction.Message.Reference.MessageId.Value);

            await interaction.Message.DeleteAsync();
            await repliedIMessage.DeleteAsync();
            repliedIMessage = await interaction.Channel.GetMessageAsync(repliedIMessage.Reference.MessageId.Value);

            List<IMessage> messages = new();
            IAsyncEnumerator<IReadOnlyCollection<IMessage>> e = interaction.Channel.GetMessagesAsync().GetAsyncEnumerator();
            try {
                while (await e.MoveNextAsync()) {
                    foreach (IMessage message in e.Current) {
                        messages.Add(message);
                    }
                }
            } finally { if (e != null) await e.DisposeAsync(); }

            messages = messages.Where(m => m.Reference != null && m.Reference.MessageId.IsSpecified && m.Reference.MessageId.Value == repliedIMessage.Id).ToList();

            foreach (IMessage message in messages) {
                await message.DeleteAsync();
            }

            await repliedIMessage.DeleteAsync();
            return await interaction.Channel.GetMessageAsync(interaction.Message.Reference.MessageId.Value);
        }
    }
}
