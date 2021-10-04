using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Interaction.TestingGrounds;
using Interaction.TestingGrounds.Listener;

namespace KillersLibraryTestBot.Modules {
    // Modules must be public and inherit from an IModuleBase
    public class TestingModule : ModuleBase<SocketCommandContext> {
        public InteractionListenerService InteractionListenerService { get; set; }


        [Command("Test")]
        [Summary("Tests the interaction listener thingy.")]
        [RequireOwner]
        public async Task ListenerTest(int num = 5) {
            // Generates a starting embed
            Func<EmbedBuilder> EmbedGenerator = () => new EmbedBuilder().WithTitle("Test");

            // Prints the current number out.
            InteractionHandlerWithEmbedder print = async (par, embed) => {
                await Task.CompletedTask;

                embed.WithDescription(new StringBuilder()
                    .AppendLine($"Current value is {num}")
                    .AppendLine($"Last button clicked was `{par?.CustomId}`")
                    .ToString());

                return InteractionResult.FromSuccess();
            };

            // Adds 1 to the number.
            InteractionHandlerWithEmbedder add = async (par, embed) => {
                ++num;

                return await print(par, embed);
            };

            // Removes 1 from the number.
            InteractionHandlerWithEmbedder sub = async (par, embed) => {
                --num;

                return await print(par, embed);
            };

            // Resets number.
            InteractionHandlerWithEmbedder reset = async (par, embed) => {
                num = 0;

                return await print(par, embed);
            };


            // Create the first page
            var embed = EmbedGenerator();
            await print(null, embed);


            var listener = new CustomInteractionListener(EmbedGenerator);

            var msg = await listener
                .WithHelpPage(new EmbedBuilder().WithTitle("help").WithDescription("????????").Build())
                .AddOption(ButtonBuilder.CreatePrimaryButton("+1", "Add"), add)
                .AddOption(ButtonBuilder.CreatePrimaryButton("=0", "Reset"), reset)
                .AddOption(ButtonBuilder.CreatePrimaryButton("-1", "Sub"), sub)
                .Create(InteractionListenerService, Context.Channel, target: Context.User, embed: embed)
                ;
        }
    }
}
