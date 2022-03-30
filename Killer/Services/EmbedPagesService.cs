using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;
using Discord;
using KillersLibrary.Utilities;

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
                await CommonService.Instance.MakeResponseAsync("error: EMBEDBUILDERS_NOT_FOUND. You didn't specify any embedBuilders to me. See Examples: https://github.com/killerfrienddk/Discord.KillersLibrary.Labs", context: context, command: command);
                return;
            }

            int currentPage = 0;
            if (styles.PageNumbers) embedBuilders[0] = embedBuilders[0].WithFooter($"Page: {currentPage + 1}/{embedBuilders.Count}");

            ComponentBuilder componentBuilder = GetComponentBuilder(embedBuilders.Count, currentPage + 1, styles, extraButtons);

            var currentMessage = await CommonService.Instance.MakeResponseAsync(embed: embedBuilders[0].Build(), component: componentBuilder.Build(), context: context, command: command);

            if (styles.ButtonDuration > 0)
            {
                var timerHandle = SetTimeout(async () =>
                {
                    await currentMessage.ModifyAsync(msg => {
                        msg.Embed = embedBuilders[currentPage].Build();

                        ComponentBuilder componentBuilderEdit = DisableEmbedButtons(embedBuilders, currentPage + 1, currentMessage, styles, extraButtons);

                        msg.Components = componentBuilderEdit.Build();
                    });
                }, styles.ButtonDuration);

                client.InteractionCreated += async (socketInteraction) => {
                    SocketMessageComponent interaction = (SocketMessageComponent)socketInteraction;
                    if (interaction.Data.Type != ComponentType.Button) return;

                    if (interaction.Message.Id == currentMessage.Id && interaction.User.Id == CommonService.Instance.GetAuthorID(context, command))
                    {
                        currentPage = await FinishEmbedActions(interaction, embedBuilders, currentPage, currentMessage, styles, extraButtons);
                    }

                    timerHandle.Dispose();

                    timerHandle = SetTimeout(async () =>
                    {
                        await currentMessage.ModifyAsync(msg => {
                            msg.Embed = embedBuilders[currentPage].Build();

                            ComponentBuilder componentBuilderEdit = DisableEmbedButtons(embedBuilders, currentPage + 1, currentMessage, styles, extraButtons);

                            msg.Components = componentBuilderEdit.Build();
                        });
                    }, styles.ButtonDuration);
                };
            }
            else
            {
                client.InteractionCreated += async (socketInteraction) => {
                    SocketMessageComponent interaction = (SocketMessageComponent)socketInteraction;
                    if (interaction.Data.Type != ComponentType.Button) return;

                    if (interaction.Message.Id == currentMessage.Id && interaction.User.Id == CommonService.Instance.GetAuthorID(context, command))
                    {
                        currentPage = await FinishEmbedActions(interaction, embedBuilders, currentPage, currentMessage, styles, extraButtons);
                    }
                };
            }
        }

        private async Task<int> FinishEmbedActions(SocketMessageComponent interaction, List<EmbedBuilder> embedBuilders, int currentPage, RestUserMessage currentMessage, EmbedPagesStyles styles, ButtonBuilder[] extraButtons) {
            currentPage = GetCurrentPage(interaction, currentPage, embedBuilders);

            switch (interaction.Data.CustomId) {
                case "killer_first_embed":
                case "killer_back_button_embed":
                case "killer_forward_button_embed":
                case "killer_last_embed":
                    if (styles.PageNumbers) embedBuilders[currentPage] = embedBuilders[currentPage].WithFooter($"Page: {currentPage + 1}/{embedBuilders.Count}");

                    await currentMessage.ModifyAsync(msg => {
                        msg.Embed = embedBuilders[currentPage].Build();

                        ComponentBuilder componentBuilder = GetComponentBuilder(embedBuilders.Count, currentPage + 1, styles, extraButtons);

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

        private ComponentBuilder DisableEmbedButtons(List<EmbedBuilder> embedBuilders, int currentPage, RestUserMessage currentMessage, EmbedPagesStyles styles, ButtonBuilder[] extraButtons)
        {
            int buttonCount = 2;

            ComponentBuilder componentBuilder = new();
            ButtonBuilder buttonBuilder;

            if (styles.FastChangeBtns)
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_first_embed")
                    .WithLabel(styles.FirstLabel ?? "«")
                    .WithStyle(styles.SkipBtnColor);
                buttonBuilder.IsDisabled = true;
                componentBuilder.WithButton(buttonBuilder);
                buttonCount++;
            }

            buttonBuilder = new ButtonBuilder()
                .WithCustomId("killer_back_button_embed")
                .WithLabel(styles.BackLabel ?? "‹")
                .WithStyle(styles.BtnColor);
            buttonBuilder.IsDisabled = true;
            componentBuilder.WithButton(buttonBuilder);

            if (!styles.RemoveDeleteBtn)
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_delete_embed_pages")
                    .WithEmote(new Emoji(styles.DeletionEmoji ?? "🗑"))
                    .WithStyle(styles.DeletionBtnColor);
                buttonBuilder.IsDisabled = true;
                componentBuilder.WithButton(buttonBuilder);
                buttonCount++;
            }

            buttonBuilder = new ButtonBuilder()
                .WithCustomId("killer_forward_button_embed")
                .WithLabel(styles.ForwardLabel ?? "›")
                .WithStyle(styles.BtnColor);
            buttonBuilder.IsDisabled = true;
            componentBuilder.WithButton(buttonBuilder);

            if (styles.FastChangeBtns)
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_last_embed")
                    .WithLabel(styles.LastLabel ?? "»")
                    .WithStyle(styles.SkipBtnColor);
                buttonBuilder.IsDisabled = true;
                componentBuilder.WithButton(buttonBuilder);
                buttonCount++;
            }

            if (extraButtons == null) return componentBuilder;

            const int maxButtonCount = 25;

            Preconditions.AtMost(buttonCount + extraButtons.Length, maxButtonCount, "Button Count", $"Please make sure that there is only {maxButtonCount} buttons!");
            for (int i = 0; i < extraButtons.Length; i++)
            {
                componentBuilder.WithButton(extraButtons[i]);
                extraButtons[i].IsDisabled = true;
            }

            return componentBuilder;
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

        private ComponentBuilder GetComponentBuilder(int maxPage, int currentPage, EmbedPagesStyles styles, ButtonBuilder[] extraButtons = null)
        {
            int buttonCount = 2;

            ComponentBuilder componentBuilder = new();
            ButtonBuilder buttonBuilder;

            if (styles.FastChangeBtns)
            {
                if (currentPage != 1)
                {
                    buttonBuilder = new ButtonBuilder()
                        .WithCustomId("killer_first_embed")
                        .WithLabel(styles.FirstLabel ?? "«")
                        .WithStyle(styles.SkipBtnColor);

                    componentBuilder.WithButton(buttonBuilder);
                    buttonCount++;
                }
                else
                {
                    buttonBuilder = new ButtonBuilder()
                        .WithCustomId("killer_first_embed")
                        .WithLabel(styles.FirstLabel ?? "«")
                        .WithStyle(styles.SkipBtnColor);

                    buttonBuilder.IsDisabled = true;

                    componentBuilder.WithButton(buttonBuilder);
                    buttonCount++;
                }
            }

            if (currentPage != 1)
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_back_button_embed")
                    .WithLabel(styles.BackLabel ?? "‹")
                    .WithStyle(styles.BtnColor);

                componentBuilder.WithButton(buttonBuilder);
            }
            else
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_back_button_embed")
                    .WithLabel(styles.BackLabel ?? "‹")
                    .WithStyle(styles.BtnColor);

                buttonBuilder.IsDisabled = true;

                componentBuilder.WithButton(buttonBuilder);
            }

            if (!styles.RemoveDeleteBtn)
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_delete_embed_pages")
                    .WithEmote(new Emoji(styles.DeletionEmoji ?? "🗑"))
                    .WithStyle(styles.DeletionBtnColor);

                componentBuilder.WithButton(buttonBuilder);
                buttonCount++;
            }

            if (currentPage < maxPage)
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_forward_button_embed")
                    .WithLabel(styles.ForwardLabel ?? "›")
                    .WithStyle(styles.BtnColor);

                componentBuilder.WithButton(buttonBuilder);
            }
            else
            {
                buttonBuilder = new ButtonBuilder()
                    .WithCustomId("killer_forward_button_embed")
                    .WithLabel(styles.ForwardLabel ?? "›")
                    .WithStyle(styles.BtnColor);

                buttonBuilder.IsDisabled = true;

                componentBuilder.WithButton(buttonBuilder);
            }

            if (styles.FastChangeBtns)
            {
                if (currentPage < maxPage)
                {
                    buttonBuilder = new ButtonBuilder()
                        .WithCustomId("killer_last_embed")
                        .WithLabel(styles.LastLabel ?? "»")
                        .WithStyle(styles.SkipBtnColor);

                    componentBuilder.WithButton(buttonBuilder);
                    buttonCount++;
                }
                else
                {
                    buttonBuilder = new ButtonBuilder()
                        .WithCustomId("killer_last_embed")
                        .WithLabel(styles.LastLabel ?? "»")
                        .WithStyle(styles.SkipBtnColor);

                    buttonBuilder.IsDisabled = true;

                    componentBuilder.WithButton(buttonBuilder);
                    buttonCount++;
                }
            }

            if (extraButtons == null) return componentBuilder;

            const int maxButtonCount = 25;

            Preconditions.AtMost(buttonCount + extraButtons.Length, maxButtonCount, "Button Count", $"Please make sure that there is only {maxButtonCount} buttons!");
            for (int i = 0; i < extraButtons.Length; i++)
            {
                componentBuilder.WithButton(extraButtons[i]);
            }

            return componentBuilder;
        }

        public static System.IDisposable SetTimeout(System.Action method, int delayInMilliseconds)
        {
            System.Timers.Timer timer = new(delayInMilliseconds);

            timer.Elapsed += (source, e) =>
            {
                method();
            };

            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Start();

            return timer;
        }
    }

    public class EmbedPagesStyles
    {
        public string FirstLabel { get; set; } = "«";
        public string BackLabel { get; set; } = "‹";
        public string DeletionEmoji { get; set; } = "🗑";
        public bool RemoveDeleteBtn { get; set; } = false;
        public string ForwardLabel { get; set; } = "›";
        public string LastLabel { get; set; } = "»";
        public string DeletionMessage { get; set; } = "Embed page has been deleted";
        public ButtonStyle BtnColor { get; set; } = ButtonStyle.Success;
        public ButtonStyle DeletionBtnColor { get; set; } = ButtonStyle.Danger;
        public ButtonStyle SkipBtnColor { get; set; } = ButtonStyle.Primary;
        public bool FastChangeBtns { get; set; } = false;
        public bool PageNumbers { get; set; } = true;
        public int ButtonDuration { get; set; } = 120000;
    }
}
