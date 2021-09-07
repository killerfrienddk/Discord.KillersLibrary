using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
using Discord;

namespace KillersLibrary.EmbedPages {
    public class EmbedPagesService {
        /// <summary>
        ///     Creates Embed Pages as a message.
        /// </summary>
        /// <param name="client">Discord Client. <see cref="DiscordSocketClient"/></param>
        /// <param name="message">Used to send the message. <see cref="SocketUserMessage"/></param>
        /// <param name="embedBuilders">Embeds that you want to be displayed as pages. <see cref="EmbedBuilder"/></param>
        /// <param name="styles">Styling or customization of the embeds and the buttons. <see cref="EmbedPagesStyles"/></param>
        public async Task CreateEmbedPages(DiscordSocketClient client, SocketUserMessage message, List<EmbedBuilder> embedBuilders, EmbedPagesStyles styles = null) {
            styles = styles ?? new();
            if (!embedBuilders.Any()) {
                await message.Channel.SendMessageAsync($"error: EMBEDBUILDERS_NOT_FOUND. You didnt specify any embedBuilders to me. See Examples: https://github.com/killerfrienddk/Discord.KillersLibrary.Labs");
                return;
            }

            ComponentBuilder componentBuilder = new();
            if (styles.FastChangeBtns) {
                ButtonBuilder firstbtn = new ButtonBuilder()
                    .WithCustomId("first_embed")
                    .WithLabel(styles.FirstLabel ?? "«")
                    .WithStyle(styles.Skipcolor);
                componentBuilder.WithButton(firstbtn);
            }

            ButtonBuilder pageMovingButtons2 = new ButtonBuilder()
                .WithCustomId("back_button_embed")
                .WithLabel(styles.BackLabel ?? "‹")
                .WithStyle(styles.Btncolor);
            componentBuilder.WithButton(pageMovingButtons2);

            ButtonBuilder deleteBtn = new ButtonBuilder()
                .WithCustomId("delete_embed_pages")
                .WithEmote(new Emoji(styles.DelEmoji ?? "🗑"))
                .WithStyle(ButtonStyle.Danger);
            componentBuilder.WithButton(deleteBtn);

            ButtonBuilder pageMovingButtons1 = new ButtonBuilder()
                .WithCustomId("forward_button_embed")
                .WithLabel(styles.ForwardLabel ?? "›")
                .WithStyle(styles.Btncolor);
            componentBuilder.WithButton(pageMovingButtons1);

            if (styles.FastChangeBtns) {
                ButtonBuilder lastbtn = new ButtonBuilder()
                    .WithCustomId("last_embed")
                    .WithLabel(styles.LastLabel ?? "»")
                    .WithStyle(styles.Skipcolor);
                componentBuilder.WithButton(lastbtn);
            }

            var currentPage = 0;
            if (styles.PageNumbers) embedBuilders[0] = embedBuilders[0].WithFooter("Page: " + (currentPage + 1) + "/" + embedBuilders.Count);
            var currentMessage = await message.Channel.SendMessageAsync(embed: embedBuilders[0].Build(), component: componentBuilder.Build());
            client.InteractionCreated += async (socketInteraction) => {
                SocketMessageComponent interaction = (SocketMessageComponent)socketInteraction;
                if (interaction.Data.Type != ComponentType.Button) return;

                if (interaction.Message.Id == currentMessage.Id && interaction.User.Id == message.Author.Id) {
                    switch (interaction.Data.CustomId) {
                        case "back_button_embed":
                            if (currentPage - 1 < 0) currentPage = embedBuilders.Count - 1;
                            else currentPage -= 1;
                            break;
                        case "forward_button_embed":
                            if (currentPage + 1 == embedBuilders.Count) currentPage = 0;
                            else currentPage += 1;
                            break;
                        case "last_embed":
                            currentPage = embedBuilders.Count - 1;
                            break;
                        case "first_embed":
                            currentPage = 0;
                            break;
                    }

                    switch (interaction.Data.CustomId) {
                        case "first_embed":
                        case "back_button_embed":
                        case "forward_button_embed":
                        case "last_embed":
                            if (styles.PageNumbers) embedBuilders[currentPage] = embedBuilders[0].WithFooter("Page: " + (currentPage + 1) + "/" + embedBuilders.Count);
                            await currentMessage.ModifyAsync(msg => {
                                msg.Embed = embedBuilders[currentPage].Build();
                                msg.Components = componentBuilder.Build();
                            });
                            break;
                        case "delete_embed_pages":
                            await currentMessage.DeleteAsync();
                            await interaction.FollowupAsync(styles.DeletionMessage, ephemeral: true);
                            break;
                    }
                }
            };
        }
    }



    public class EmbedPagesStyles {
        public string FirstLabel { get; set; } = "«";
        public string BackLabel { get; set; } = "‹";
        public string DelEmoji { get; set; } = "🗑";
        public string ForwardLabel { get; set; } = "›";
        public string LastLabel { get; set; } = "»";
        public string DeletionMessage { get; set; } = "Embed page has been deleted";
        public ButtonStyle Btncolor { get; set; } = ButtonStyle.Success;
        public ButtonStyle Delcolor { get; set; } = ButtonStyle.Danger;
        public ButtonStyle Skipcolor { get; set; } = ButtonStyle.Primary;
        public bool FastChangeBtns { get; set; } = false;
        public bool PageNumbers { get; set; } = true;
    }
}
