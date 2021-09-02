using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
using Discord;

namespace Killer.EmbedPages {
    public class EmbedPagesService {
        public async Task CreateEmbedPages(DiscordSocketClient client, SocketUserMessage message, List<EmbedBuilder> embedBuilders, EmbedPagesStyles style = null) {
            if (style == null) style = new();
            if (!embedBuilders.Any()) {
                await message.Channel.SendMessageAsync($"error: EMBEDBUILDERS_NOT_FOUND. You didnt specify any embedBuilders to me. See Examples: ");
                return;
            }

            ComponentBuilder componentBuilder = new();
            if (style.FastChangeBtns) {
                ButtonBuilder firstbtn = new ButtonBuilder()
                    .WithCustomId("first_embed")
                    .WithLabel(style.FirstLabel ?? "«")
                    .WithStyle(style.Skipcolor);
                componentBuilder.WithButton(firstbtn);
            }

            ButtonBuilder pageMovingButtons2 = new ButtonBuilder()
                .WithCustomId("back_button_embed")
                .WithLabel(style.BackLabel ?? "‹")
                .WithStyle(style.Btncolor);
            componentBuilder.WithButton(pageMovingButtons2);

            ButtonBuilder deleteBtn = new ButtonBuilder()
                .WithCustomId("delete")
                .WithEmote(new Emoji(style.DelEmoji ?? "🗑"))
                .WithStyle(ButtonStyle.Danger);
            componentBuilder.WithButton(deleteBtn);

            ButtonBuilder pageMovingButtons1 = new ButtonBuilder()
                .WithCustomId("forward_button_embed")
                .WithLabel(style.ForwardLabel ?? "›")
                .WithStyle(style.Btncolor);
            componentBuilder.WithButton(pageMovingButtons1);

            if (style.FastChangeBtns) {
                ButtonBuilder lastbtn = new ButtonBuilder()
                    .WithCustomId("last_embed")
                    .WithLabel(style.LastLabel ?? "»")
                    .WithStyle(style.Skipcolor);
                componentBuilder.WithButton(lastbtn);
            }

            var currentPage = 0;
            if (style.PageNumbers) embedBuilders[0] = embedBuilders[0].WithFooter("Page: " + (currentPage + 1) + "/" + embedBuilders.Count);
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
                            if (style.PageNumbers) embedBuilders[currentPage] = embedBuilders[0].WithFooter("Page: " + (currentPage + 1) + "/" + embedBuilders.Count);
                            await currentMessage.ModifyAsync(msg => {
                                msg.Embed = embedBuilders[currentPage].Build();
                                msg.Components = componentBuilder.Build();
                            });
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
        public ButtonStyle Btncolor { get; set; } = ButtonStyle.Success;
        public ButtonStyle Delcolor { get; set; } = ButtonStyle.Danger;
        public ButtonStyle Skipcolor { get; set; } = ButtonStyle.Primary;
        public bool FastChangeBtns { get; set; } = false;
        public bool PageNumbers { get; set; } = true;
    }
}
