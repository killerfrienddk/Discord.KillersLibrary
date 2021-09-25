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
        /// <param name="context">the <see cref="SocketCommandContext"/> used to send normal commands.</param>
        /// <param name="context">the <see cref="SocketSlashCommand"/> used to send slash commands.</param>
        /// <param name="styles">The <see cref="EmbedPagesStyles"/> is for customization of many parameters.</param>
        public virtual async Task CreateEmbedPages(DiscordSocketClient client, List<EmbedBuilder> embedBuilders, SocketCommandContext context = null, SocketSlashCommand command = null, EmbedPagesStyles styles = null) {
            CommonService.Instance.ContextAndCommandIsNullCheck(context, command);
            if (context == null) await command.DeferAsync();

            styles ??= new();
            if (!embedBuilders.Any()) {
                await CommonService.Instance.MakeResponse("error: EMBEDBUILDERS_NOT_FOUND. You didnt specify any embedBuilders to me. See Examples: https://github.com/killerfrienddk/Discord.KillersLibrary.Labs", context: context, command: command);
                return;
            }

            ComponentBuilder componentBuilder = GetComponentBuilder(styles);

            int currentPage = 0;
            if (styles.PageNumbers) embedBuilders[0] = embedBuilders[0].WithFooter($"Page: {currentPage + 1}/{embedBuilders.Count}");
            var currentMessage = await CommonService.Instance.MakeResponse(embed: embedBuilders[0].Build(), component: componentBuilder.Build(), context: context, command: command);
            client.InteractionCreated += async (socketInteraction) => {
                SocketMessageComponent interaction = (SocketMessageComponent)socketInteraction;
                if (interaction.Data.Type != ComponentType.Button) return;

                if (interaction.Message.Id == currentMessage.Id && interaction.User.Id == CommonService.Instance.GetAuthorID(context, command)) {
                    currentPage = await FinishEmbedActions(interaction, embedBuilders, currentPage, currentMessage, componentBuilder, styles);
                }
            };
        }

        #region Obsolete
        /// <summary>
        ///     Creates Embed Pages as a message.
        /// </summary>
        /// <param name="client">Discord Client. <see cref="DiscordSocketClient"/></param>
        /// <param name="message">Used to send the message. <see cref="SocketUserMessage"/></param>
        /// <param name="embedBuilders">Embeds that you want to be displayed as pages. <see cref="EmbedBuilder"/></param>
        /// <param name="styles">Styling or customization of the embeds and the buttons. <see cref="EmbedPagesStyles"/></param>
        [Obsolete("This method will soon be deprecated and will be removed in future versions. Please use the new version instead which is called the same but it uses the context and or the slashcommands.")]
        public virtual async Task CreateEmbedPages(DiscordSocketClient client, SocketUserMessage message, List<EmbedBuilder> embedBuilders, EmbedPagesStyles styles = null) {
            styles ??= new();

            if (!embedBuilders.Any()) {
                await message.Channel.SendMessageAsync($"error: EMBEDBUILDERS_NOT_FOUND. You didnt specify any embedBuilders to me. See Examples: https://github.com/killerfrienddk/Discord.KillersLibrary.Labs");
                return;
            }

            ComponentBuilder componentBuilder = GetComponentBuilder(styles);

            var currentPage = 0;
            if (styles.PageNumbers) embedBuilders[0] = embedBuilders[0].WithFooter("Page: " + (currentPage + 1) + "/" + embedBuilders.Count);
            var currentMessage = await message.Channel.SendMessageAsync(embed: embedBuilders[0].Build(), component: componentBuilder.Build());
            client.InteractionCreated += async (socketInteraction) => {
                SocketMessageComponent interaction = (SocketMessageComponent)socketInteraction;
                if (interaction.Data.Type != ComponentType.Button) return;

                if (interaction.Message.Id == currentMessage.Id && interaction.User.Id == message.Author.Id) {
                    currentPage = await FinishEmbedActions(interaction, embedBuilders, currentPage, currentMessage, componentBuilder, styles);
                }
            };
        }
        #endregion

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

        private ComponentBuilder GetComponentBuilder(EmbedPagesStyles styles) {
            ComponentBuilder componentBuilder = new();
            if (styles.FastChangeBtns) {
                ButtonBuilder firstBtn = new ButtonBuilder()
                    .WithCustomId("killer_first_embed")
                    .WithLabel(styles.FirstLabel ?? "«")
                    .WithStyle(styles.SkipColor);
                componentBuilder.WithButton(firstBtn);
            }

            ButtonBuilder pageMovingButtons2 = new ButtonBuilder()
                .WithCustomId("killer_back_button_embed")
                .WithLabel(styles.BackLabel ?? "‹")
                .WithStyle(styles.BtnColor);
            componentBuilder.WithButton(pageMovingButtons2);

            ButtonBuilder deleteBtn = new ButtonBuilder()
                .WithCustomId("killer_delete_embed_pages")
                .WithEmote(new Emoji(styles.DelEmoji ?? "🗑"))
                .WithStyle(ButtonStyle.Danger);
            componentBuilder.WithButton(deleteBtn);

            ButtonBuilder pageMovingButtons1 = new ButtonBuilder()
                .WithCustomId("killer_forward_button_embed")
                .WithLabel(styles.ForwardLabel ?? "›")
                .WithStyle(styles.BtnColor);
            componentBuilder.WithButton(pageMovingButtons1);

            if (styles.FastChangeBtns) {
                ButtonBuilder lastBtn = new ButtonBuilder()
                    .WithCustomId("killer_last_embed")
                    .WithLabel(styles.LastLabel ?? "»")
                    .WithStyle(styles.SkipColor);
                componentBuilder.WithButton(lastBtn);
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

    public enum ColorCodes : uint {
        Default = 0x7F7F7F,
        Black_Absolute = 0x000000,
        Black = 0x0C0C0C,
        Grey_Dark = 0x3F3F3F,
        Grey = 0x7F7F7F,
        Grey_Light = 0xD3D3D3,
        White = 0xF2F2F2,
        White_Absolute = 0xFFFFFF,
        Red_Light = 0xFF3333,
        Red = 0xFF0000,
        Red_Dark = 0x8C0000,
        Green_Light = 0x8EED8E,
        Green = 0x007F00,
        Green_Dark = 0x006300,
        Blue_Light = 0xADD8E5,
        Blue = 0x0000FF,
        Blue_Dark = 0x00008C,
        Yellow_Light = 0xFF0000,
        Yellow = 0xFFCC00,
        Yellow_Dark = 0xFFFFCC,
        Orange_Light = 0xD8A366,
        Orange = 0xFFA500,
        Orange_Dark = 0xFF8C00,
        Purple_Light = 0x9370DB,
        Purple = 0xA021EF,
        Purple_Dark = 0x663399,
        Pink_Light = 0x000000,
        Pink = 0xFFBFCC,
        Pink_Dark = 0xE8547F,
        Cyan_Light = 0xE0FFFF,
        Cyan = 0x00FFFF,
        Cyan_Dark = 0x008C8C,
        Magenta_Light = 0xFF77FF,
        Magenta = 0xFF00FF,
        Magenta_Dark = 0x8C008C,
        Brown_Light = 0xAF7F51,
        Brown = 0xA52828,
        Brown_Dark = 0x5B3F33,
        Violet_Light = 0x7A5199,
        Violet = 0xED82ED,
        Violet_Dark = 0x9300D3,
        Turquoise_Light = 0xAFE2DD,
        Turquoise = 0x30D6C6,
        Turquoise_Dark = 0x00CED1,
        Mustard_Light = 0xEDDD60,
        Mustard = 0xFFDB59,
        Mustard_Dark = 0x7C7C3F,
        Ivory_Light = 0xFFF7C9,
        Ivory = 0xFFFFEF,
        Ivory_Dark = 0xF2E58E,
        Gold_Light = 0xF2E5AA,
        Gold = 0xFFD600,
        Gold_Dark = 0xEDBC1C,
        Silver_Light = 0xE0E0E0,
        Silver = 0xBFBFBF,
        Silver_Dark = 0xAFAFAF,
        Silver_Crayola = 0xC9C0BB,
        Silver_Pink = 0xC4AEAD,
        Silver_Sand = 0xBFC1C2,
        Silver_Chalice = 0xACACAC,
        Silver_Roman = 0x838996,
        Silver_Old = 0x848482,
        Silver_Sonic = 0x757575,
        Bronze = 0xCD7F32,
        Bronze_BlastOff = 0xA57164,
        Bronze_Antique = 0x665D1E,
        Lime = 0x00FF00,
        Teal = 0x007F7F,
        Aquamarine = 0x7FFFD3,
        Aqua = 0x00FFFF,
        Beige = 0xF4F4DB,
        Crimson = 0xDB143D,
        Coral = 0xFF7F4F,
        Deep_Sky_Blue = 0x00BFFF,
        Dodger_Blue = 0x1E90FF,
        Firebrick = 0xB22222,
        Fuchsia = 0xFF00FF,
        Indigo = 0x4B0082,
        Light_Sky_Blue = 0x87CEFA,
        Maroon = 0xB03060,
    }
}