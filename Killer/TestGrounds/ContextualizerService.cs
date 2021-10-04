using System.Collections.Immutable;
using System.Threading.Tasks;
using System;

using Discord.WebSocket;
using Discord.Rest;
using Discord.Net;
using Discord;

using Interaction.TestingGrounds.Extensions;
using Interaction.TestingGrounds.Parameter;

namespace Interaction.TestingGrounds.Services {
    public partial class ContextualizerService {
        protected bool Running { get; set; } = false;

        protected DiscordSocketClient client;

        public static IServiceProvider Services { get; set; }

        public ContextualizerService(IServiceProvider services) {
            Console.WriteLine($"[ContextualizerService.ContextualizerService()] ");
            Services = services;
        }

        public async Task StartAsync(DiscordSocketClient c) {
            await Task.CompletedTask;
            if (Running) {
                //Console.WriteLine($"[ContextualizerService.StartAsync()] Already running");
                return;
            }
            Running = true;
            client = c;
            Console.WriteLine($"[ContextualizerService.StartAsync()] Start");
            client.InteractionCreated += InteractionCreatedEvent;

            // Test
            //client.Ready += Client_Ready;

            //await Client_Ready();


            client.UserCommandExecuted += Handler_UserCommandExecuted;
            client.MessageCommandExecuted += Handler_MessageCommandExecuted;
            client.SlashCommandExecuted += Handler_SlashCommandExecuted;

            Console.WriteLine($"[ContextualizerService.StartAsync()] End");
            //return Task.CompletedTask;
        }

        // ---- // Event handlers
        protected async Task Handler_InteractionCreated(SocketInteraction interaction) {
            await SysLog.Instance.Verbose(GetType(), $"Id: {interaction.Id}, Channel: {interaction.Channel.Name}, User: {interaction.User}, Type: {interaction.Type}, Valid: {interaction.IsValidToken}, Timestamp: {interaction.CreatedAt.ToString()}, Version: {interaction.Version}"); // , Token: {interaction.Token}

            switch (interaction) {
                // If this is a component interaction
                case SocketMessageComponent comp:
                    await Handler_ComponentInteraction(comp);
                    break;

                // If this is a slash command:
                case SocketSlashCommand command:
                    await Handler_SlashCommand(command);
                    break;

                // If interaction type isn't recognized
                default:
                    _ = InteractionCreated?.Invoke(interaction);
                    break;
            }
        }

        protected async Task Handler_ComponentInteraction(SocketMessageComponent comp) {
            var data = comp.Data;
            var component = comp.Message.Components.GetComponent(comp.Data.CustomId);
            //var component = getTargetComponent(comp.Message.Components, comp.Data.CustomId);
            var values = data.Values ?? ImmutableList<string>.Empty;

            // Create parameter object
            MessageComponentParams par;

            // Create the appropriate parameter type
            switch (component) {
                case SelectMenuComponent menu: {
                        par = new MenuSelectionParams {
                            MessageComponent = comp,
                            Interaction = comp,
                            Component = menu,
                            Options = menu.GetSelectedOptions(values),
                        };
                        break;
                    }
                case ButtonComponent button: {
                        par = new ButtonClickParams {
                            MessageComponent = comp,
                            Interaction = comp,
                            Component = button,
                        };
                        break;
                    }
                default: {
                        par = new MessageComponentParams {
                            MessageComponent = comp,
                            Interaction = comp,
                            Component = component,
                        };
                        break;
                    }
            }

            // Set the guild if found.
            if (comp.Channel is IGuildChannel gc) await par.SetGuild(gc.Guild);

            // Invoke the managed event
            _ = MessageComponentInteraction?.Invoke(par);
        }

        protected async Task Handler_SlashCommand(SocketSlashCommand command) {
            // Create params object
            SlashCommandParams par = new SlashCommandParams {
                SlashCommand = command,
                Interaction = command,
            };
            if (command.Channel is IGuildChannel gc) await par.SetGuild(gc.Guild);

            // Start event.
            _ = SlashCommandInteraction?.Invoke(par);
        }


        protected async Task Handler_UserCommandExecuted(SocketUserCommand command) {
            await Task.CompletedTask;

            _ = UserCommandExecuted?.Invoke(command);
        }
        protected async Task Handler_MessageCommandExecuted(SocketMessageCommand command) {
            await Task.CompletedTask;

            _ = MessageCommandExecuted?.Invoke(command);
        }
        protected async Task Handler_SlashCommandExecuted(SocketSlashCommand command) {
            await Task.CompletedTask;

            _ = SlashCommandExecuted?.Invoke(command);
        }

        // ---- // Event handlers
        protected async Task InteractionCreatedEvent(SocketInteraction interaction) {
            _ = Handler_InteractionCreated(interaction);
            await Task.CompletedTask;
        }

        // ---- // Event
        public event Func<SocketInteraction, Task> InteractionCreated;
        public event Func<MessageComponentParams, Task> MessageComponentInteraction;
        public event Func<SlashCommandParams, Task> SlashCommandInteraction;

        public event Func<SocketUserCommand, Task> UserCommandExecuted;
        public event Func<SocketMessageCommand, Task> MessageCommandExecuted;
        public event Func<SocketSlashCommand, Task> SlashCommandExecuted;


    }



    /*
		Testing stuff.
	*/
    public partial class ContextualizerService {

        /*
			When we're ready.
		*/
        public async Task Client_Ready() {
            // Let's build a guild command! We're going to need a guild id so lets just put that in a variable.
            ulong guildId = 272837808098639873;

            // Next, lets create our user and message command builder. This is like the embed builder but for context menu commands.
            //var guildUserCommand = new UserCommandBuilder();
            //var guildMessageCommand = new MessageCommandBuilder();

            // Note: Names have to be all lowercase and match the regular expression ^[\w -]{3,32}$
            //guildUserCommand.WithName("Guild User Command");
            //guildMessageCommand.WithName("Guild Message Command");

            // Descriptions are not used with User and Message commands
            //guildCommand.WithDescription("");

            // Let's do our global commands
            var globalCommand = new UserCommandBuilder()
                .WithName("Info");
            var guildCommand = new UserCommandBuilder()
                .WithName("Info2");

            //var globalMessageCommand = new MessageCommandBuilder()
            //	.WithName("Global Message Command");


            var gComs = await client.Rest.GetGlobalApplicationCommands();
            var gComs2 = await client.Rest.GetGuildApplicationCommands(guildId);


            RestGlobalCommand a = null;
            RestGuildCommand b = null;

            try {
                a = await client.Rest.CreateGlobalCommand(globalCommand.Build(), new RequestOptions {
                    Timeout = 5_000,
                });
            } catch (ApplicationCommandException exception) {
                await SysLog.Instance.Error(GetType(), exception: exception);
            }
            try {
                b = await client.Rest.CreateGuildCommand(guildCommand.Build(), guildId, new RequestOptions {
                    Timeout = 5_000,
                });
            } catch (ApplicationCommandException exception) {
                await SysLog.Instance.Error(GetType(), exception: exception);
            }

            await SysLog.Instance.Info(GetType(), $"Registered");
        }
    }
}