using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using KillersLibrary.Services;

namespace KillersLibraryTestBot.Services.Commands {
    public class CommandsService {
        private readonly EmbedPagesService _embedPagesService;
        private readonly PictureService _pictureService;

        public CommandsService(EmbedPagesService embedPagesService, PictureService pictureService) {
            _embedPagesService = embedPagesService;
            _pictureService = pictureService;
        }

        public async Task HelpAsync(SocketCommandContext context = null, SocketSlashCommand command = null, DiscordSocketClient client = null) {
            List<EmbedBuilder> embedBuilders = new();

            EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle("Family System")
            .AddField("News!", "The bot now works with slash commands.")
            .AddField("New Stuff!", "- Leave Family.\n\n- Remove Family Member")
            .WithDescription("This is the family system.");
            StringBuilder sb = new();
            sb.Append(" - Remove Family Connection.\n");
            sb.Append(" - Add Family Head.\n ");
            sb.Append(" - Remove Family Head .\n");
            sb.Append("Commands to help you:");
            embedBuilder.AddField("Planned Stuff:", sb.ToString())
            .AddField("/add-family, !af or !addFamily", "Add a new family.")
            .AddField("/families, !f or !families", "See families and select one to see who is in it.")
            .AddField("/join-family, !jf or !joinFamily", "Join an existing family.")
            .AddField("/add-family-member, !afm or !addFamilyMember", "Add family member.")
            .AddField("/remove-family-member, !rfm or !removeFamilyMember", "Remove a family member, you have to be a family head.")
            .AddField("/remove-family-connection, !rfc or !removeFamilyConnection", "Not implemented yet."/*"Remove a connection between you and another."*/)
            .AddField("/add-family-head, !afh or !addFamilyHead", "Not implemented yet."/*"Add a family head."*/)
            .AddField("/remove-family-head, !rfh or !removeFamilyHead", "Not implemented yet."/*"Remove a family head."*/)
            .AddField("/leave-family, !lf or !leaveFamily", "Leave a family.");
            embedBuilders.Add(embedBuilder);

            embedBuilder = new EmbedBuilder()
            .WithTitle("Animal Searches")
            .WithDescription("This is the animal picture system here is some commands to help you:")
            .AddField("/cat or !cat", "Get a cat picture.")
            .AddField("/dog or !dog", "Get a dog picture.")
            .AddField("/fox or !fox", "Get a fox picture.")
            .AddField("/panda or !panda", "Get a panda picture.")
            .AddField("/redpanda or !redpanda", "Get a redpanda picture.");
            embedBuilders.Add(embedBuilder);

            await _embedPagesService.CreateEmbedPages(client, embedBuilders, context: context, command: command);
        }

       

        #region Animal Pictures
        public async Task SelectSendAnimal(string name, SocketCommandContext context = null, SocketSlashCommand command = null) {
            (Stream, string) message = (null, null);
            switch (name) {
                case "cat":
                    message = await _pictureService.GetCatPictureAsync();
                    break;
                case "dog":
                    message = await _pictureService.GetDogPictureAsync();
                    break;
                case "panda":
                    message = await _pictureService.GetPictureFromSomeRandomApi("panda");
                    break;
                case "fox":
                    message = await _pictureService.GetPictureFromSomeRandomApi("fox");
                    break;
                case "redpanda":
                    message = await _pictureService.GetPictureFromSomeRandomApi("red_panda");
                    break;
            }

            await SendAnimal(name, message, context, command);
        }
        public async Task SendAnimal(string name, (Stream, string) message, SocketCommandContext context = null, SocketSlashCommand command = null) {
            //(Stream, string) message = await _pictureService.GetRedPandaPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            message.Item1.Seek(0, SeekOrigin.Begin);

            await CommonService.Instance.MakeFileResponseAsync(message.Item1, name + ".jpg", string.IsNullOrEmpty(message.Item2) ? null : "Random fact: " + message.Item2, context: context, command: command);
        }
        #endregion

        public async Task PingAsync(SocketCommandContext context = null, SocketSlashCommand command = null) {
            await CommonService.Instance.MakeResponseAsync("pong!", context: context, command: command);
        }
    }
}
