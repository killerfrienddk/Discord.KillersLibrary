using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;
using Discord;

namespace KillersLibrary.Services {
    public class EmbedPagesService {
        /// <summary>
        ///     Creates Embed Pages as a message.
        /// </summary>
        /// <param name="client">The <see cref="DiscordSocketClient"/> Client. </param>
        /// <param name="embedBuilders">The list of <see cref="EmbedBuilder"/> is used to display them as pages.</param>
        /// <param name="context">The <see cref="SocketCommandContext"/> used to send normal commands.</param>
        /// <param name="command">The <see cref="SocketSlashCommand"/> used to send slash commands.</param>
        /// <param name="styles">The <see cref="EmbedPagesStyles"/> is for customization of many parameters.</param>
        public virtual async Task CreateEmbedPages(DiscordSocketClient client, List<EmbedBuilder> embedBuilders, ButtonBuilder[] extraButtons = null, SocketCommandContext context = null, SocketSlashCommand command = null, EmbedPagesStyles styles = null) {
            CommonService.Instance.ContextAndCommandIsNullCheck(context, command);
            if (context == null) await command.DeferAsync();

            styles ??= new();
            if (!embedBuilders.Any()) {
                await CommonService.Instance.MakeResponseAsync("error: EMBEDBUILDERS_NOT_FOUND. You didnt specify any embedBuilders to me. See Examples: https://github.com/killerfrienddk/Discord.KillersLibrary.Labs", context: context, command: command);
                return;
            }

            ComponentBuilder componentBuilder = GetComponentBuilder(styles, extraButtons);

            int currentPage = 0;
            if (styles.PageNumbers) embedBuilders[0] = embedBuilders[0].WithFooter($"Page: {currentPage + 1}/{embedBuilders.Count}");
            var currentMessage = await CommonService.Instance.MakeResponseAsync(embed: embedBuilders[0].Build(), component: componentBuilder.Build(), context: context, command: command);
            client.InteractionCreated += async (socketInteraction) => {
                SocketMessageComponent interaction = (SocketMessageComponent)socketInteraction;
                if (interaction.Data.Type != ComponentType.Button) return;

                if (interaction.Message.Id == currentMessage.Id && interaction.User.Id == CommonService.Instance.GetAuthorID(context, command)) {
                    currentPage = await FinishEmbedActions(interaction, embedBuilders, currentPage, currentMessage, componentBuilder, styles);
                }
            };
        }

        private async Task<int> FinishEmbedActions(SocketMessageComponent interaction, List<EmbedBuilder> embedBuilders, int currentPage, RestUserMessage currentMessage, ComponentBuilder componentBuilder, EmbedPagesStyles styles) {
            currentPage = GetCurrentPage(interaction, currentPage, embedBuilders);

            switch (interaction.Data.CustomId) {
                case "killer_first_embed":
                case "killer_back_button_embed":
                case "killer_forward_button_embed":
                case "killer_last_embed":
                    if (styles.PageNumbers) embedBuilders[currentPage] = embedBuilders[currentPage].WithFooter($"Page: {currentPage + 1}/{embedBuilders.Count}");
                    await currentMessage.ModifyAsync(msg => {
                        msg.Embed = embedBuilders[currentPage].Build();
                        msg.Components = componentBuilder.Build();
                    });
                    break;
                case "killer_delete_embed_pages":
                    await currentMessage.DeleteAsync();
                    await interaction.FollowupAsync(styles.DeletionMessage, ephemeral: true);
                    break;
            }

            return currentPage;
        }

        private int GetCurrentPage(SocketMessageComponent interaction, int currentPage, List<EmbedBuilder> embedBuilders) {
            switch (interaction.Data.CustomId) {
                case "killer_back_button_embed":
                    if (currentPage - 1 < 0) currentPage = embedBuilders.Count - 1;
                    else currentPage -= 1;
                    break;
                case "killer_forward_button_embed":
                    if (currentPage + 1 == embedBuilders.Count) currentPage = 0;
                    else currentPage += 1;
                    break;
                case "killer_last_embed":
                    currentPage = embedBuilders.Count - 1;
                    break;
                case "killer_first_embed":
                    currentPage = 0;
                    break;
            }

            return currentPage;
        }

        private ComponentBuilder GetComponentBuilder(EmbedPagesStyles styles, ButtonBuilder[] extraButtons = null) {
            int buttonCount = 3;
            ComponentBuilder componentBuilder = new();
            ButtonBuilder buttonBuilder;
            if (styles.FastChangeBtns) {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_first_embed")
                    .WithLabel(styles.FirstLabel ?? "«")
                    .WithStyle(styles.SkipColor);
                componentBuilder.WithButton(buttonBuilder);
                buttonCount++;
            }

            buttonBuilder = new ButtonBuilder()
            .WithCustomId("killer_back_button_embed")
                .WithLabel(styles.BackLabel ?? "‹")
                .WithStyle(styles.BtnColor);
            componentBuilder.WithButton(buttonBuilder);

            buttonBuilder = new ButtonBuilder()
            .WithCustomId("killer_delete_embed_pages")
                .WithEmote(new Emoji(styles.DelEmoji ?? "🗑"))
                .WithStyle(ButtonStyle.Danger);
            componentBuilder.WithButton(buttonBuilder);

            buttonBuilder = new ButtonBuilder()
            .WithCustomId("killer_forward_button_embed")
                .WithLabel(styles.ForwardLabel ?? "›")
                .WithStyle(styles.BtnColor);
            componentBuilder.WithButton(buttonBuilder);

            if (styles.FastChangeBtns) {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_last_embed")
                    .WithLabel(styles.LastLabel ?? "»")
                    .WithStyle(styles.SkipColor);
                componentBuilder.WithButton(buttonBuilder);
                buttonCount++;
            }

            int maxButtonCount = 25;
            Preconditions.AtMost(maxButtonCount, buttonCount + extraButtons.Length, "Button Count", $"Please make sure that there is only {maxButtonCount} buttons!");
            for (int i = 0; i < (extraButtons.Length > maxButtonCount ? maxButtonCount : extraButtons.Length); i++) {
                componentBuilder.WithButton(extraButtons[i]);
            }

            return componentBuilder;
        }
    }

    public class EmbedPagesStyles {
        public string FirstLabel { get; set; } = "«";
        public string BackLabel { get; set; } = "‹";
        public string DelEmoji { get; set; } = "🗑";
        public string ForwardLabel { get; set; } = "›";
        public string LastLabel { get; set; } = "»";
        public string DeletionMessage { get; set; } = "Embed page has been deleted";
        public ButtonStyle BtnColor { get; set; } = ButtonStyle.Success;
        public ButtonStyle Delcolor { get; set; } = ButtonStyle.Danger;
        public ButtonStyle SkipColor { get; set; } = ButtonStyle.Primary;
        public bool FastChangeBtns { get; set; } = false;
        public bool PageNumbers { get; set; } = true;
    }
}
