using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Net;
using Discord;
using KillersLibraryTestBot.Services.Commands;
using Newtonsoft.Json;

namespace KillersLibraryTestBot.Services {
    public class CommandHandlingService {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly SelectCommandsService _selectCommands;
        private readonly ButtonCommandsService _buttonCommands;
        private readonly CommandsService _commandsService;

        public CommandHandlingService(IServiceProvider services, SelectCommandsService selectCommands, ButtonCommandsService buttonCommands, CommandsService commandsService) {
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _selectCommands = selectCommands;
            _buttonCommands = buttonCommands;
            _commandsService = commandsService;

            _commands.CommandExecuted += CommandExecutedAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.InteractionCreated += Client_InteractionCreated;
            _client.Ready += Client_Ready;
        }

        public SlashCommandProperties AddSlashCommand(string name, string description = "_", SlashCommandOptionBuilder slashCommandOptionBuilder = null, List<SlashCommandOptionBuilder> slashCommandOptionBuilders = null) {
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName(name);
            globalCommand.WithDescription(description);
            if (slashCommandOptionBuilder != null)
                globalCommand.AddOption(slashCommandOptionBuilder);
            if (slashCommandOptionBuilders != null && slashCommandOptionBuilders.Any())
                globalCommand.AddOptions(slashCommandOptionBuilders.ToArray());
            return globalCommand.Build();
        }

        public async Task Client_Ready() {
            try {
                /*List<ApplicationCommandProperties> applicationCommandProperties = new();
                applicationCommandProperties.Add(AddSlashCommand("help", "Get a list of all of the commands and other info about the bot."));
                applicationCommandProperties.Add(AddSlashCommand("families", "Get a list of all of the families to choose from see who is in the family."));

                SlashCommandOptionBuilder slashCommandOptionBuilder = new();
                slashCommandOptionBuilder.WithName("name");
                slashCommandOptionBuilder.WithType(ApplicationCommandOptionType.String);
                slashCommandOptionBuilder.WithDescription("Add a family");
                slashCommandOptionBuilder.WithRequired(true);

                applicationCommandProperties.Add(AddSlashCommand("add-family", "Add a new family to the bot.", slashCommandOptionBuilder: slashCommandOptionBuilder));
                applicationCommandProperties.Add(AddSlashCommand("join-family", "Join a family."));
                applicationCommandProperties.Add(AddSlashCommand("leave-family", "Leave a family."));
                applicationCommandProperties.Add(AddSlashCommand("remove-family-member", "Remove a family member, to do this you need to the creator of the family."));
                applicationCommandProperties.Add(AddSlashCommand("add-family-member", "Add a new family member."));
                applicationCommandProperties.Add(AddSlashCommand("cat", "Get a random picture of a cat and a fact about cats."));
                applicationCommandProperties.Add(AddSlashCommand("dog", "Get a random picture of a dog and a fact about dogs."));
                applicationCommandProperties.Add(AddSlashCommand("panda", "Get a random picture of a panda and a fact about pandas."));
                applicationCommandProperties.Add(AddSlashCommand("redpanda", "Get a random picture of a redpanda and a fact about redpandas."));
                applicationCommandProperties.Add(AddSlashCommand("fox", "Get a random picture of a fox and a fact about foxes."));
                applicationCommandProperties.Add(AddSlashCommand("ping", "Get the bot to return Pong!"));

                // new commands
                *//*
                applicationCommandProperties.Add(AddSlashCommand("remove-family-connection", "Remove a connection between you and another."));
                applicationCommandProperties.Add(AddSlashCommand("add-family-head", "Add a family head."));
                applicationCommandProperties.Add(AddSlashCommand("remove-family-head", "Remove a family head."));
                *//*

                await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());*/
            } catch (ApplicationCommandException exception) {
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);

                Console.WriteLine(json);
            }

            Console.WriteLine(DateTime.Now.ToShortTimeString() + ":" + (DateTime.Now.Second.ToString().Length == 1 ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString()) + " Main        Slash Commands Loaded");
        }

        private async Task SlashCommandHandler(SocketSlashCommand command) {
            /*int argPos = 0;
            if (!command.HasStringPrefix("!", ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _services);*/
            SocketSlashCommandDataOption socketSlashCommandDataOption = command.Data.Options?.First();
            var firstValue = socketSlashCommandDataOption?.Value;

            // Let's add a switch statement for the command name so we can handle multiple commands in one event.
            switch (command.Data.Name) {
                case "help":
                    await _commandsService.HelpAsync(command: command, client: _client);
                    break;
                case "cat":
                    await _commandsService.SelectSendAnimal("cat", command: command);
                    break;
                case "dog":
                    await _commandsService.SelectSendAnimal("dog", command: command);
                    break;
                case "panda":
                    await _commandsService.SelectSendAnimal("panda", command: command);
                    break;
                case "fox":
                    await _commandsService.SelectSendAnimal("fox", command: command);
                    break;
                case "redpanda":
                    await _commandsService.SelectSendAnimal("redpanda", command: command);
                    break;
                case "ping":
                    await _commandsService.PingAsync(command: command);
                    break;
            }
        }

        private async Task MessageComponentHandler(SocketMessageComponent interaction) {
            switch (interaction.Data.Type) {
                case ComponentType.ActionRow:
                    break;
                case ComponentType.Button:
                    await ButtonHandler(interaction);
                    break;
                case ComponentType.SelectMenu:
                    await SelectMenuHandler(interaction);
                    break;
                default:
                    break;
            }
        }

        #region Select Functions
        private async Task SelectMenuHandler(SocketMessageComponent interaction) {
            // Get the custom ID 
            var customId = interaction.Data.CustomId;
            // Get the user
            var user = (SocketGuildUser)interaction.User;
            // Get the guild
            var guild = user.Guild;

            // If you are using selection dropdowns, you can get the selected label and values using these
            var selectedLabel = ((SelectMenuComponent)interaction.Message.Components.First().Components.First()).Options.FirstOrDefault(x => x.Value == interaction.Data.Values.FirstOrDefault())?.Label;
            string selectedValue = interaction.Data.Values.First();

            switch (customId) {
                case "selectParentType":
                    await _selectCommands.ChooseParentType(interaction, Convert.ToUInt64(selectedValue));
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Button Functions
        private async Task ButtonHandler(SocketMessageComponent interaction) {
            // Get the custom ID 
            var customId = interaction.Data.CustomId;
            // Get the user
            //var user = (SocketGuildUser)interaction.User;
            // Get the guild
            //var guild = user.Guild;

            /*  // Respond with the update message. This edits the message which this component resides.
              await interaction.UpdateAsync(msgProps => msgProps.Content = $"Clicked {interaction.Data.CustomId}!");
              // Also you can followup with a additional messages
              await interaction.FollowupAsync($"Clicked {interaction.Data.CustomId}!", ephemeral: true);*/

            if (Regex.IsMatch(customId, "multiButtons[0-9]+")) await _buttonCommands.ChooseChildNameRange(interaction);


            switch (customId) {
                case "delete":
                    await interaction.Message.DeleteAsync();
                    await interaction.RespondAsync("Message Deleted", ephemeral: true);
                    break;
                case "chooseChild":
                    await _buttonCommands.CreateChooseChildButtons(interaction);
                    break;
                case "cancel":
                case "goBack":
                    await interaction.Message.DeleteAsync();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Other
        private async Task Client_InteractionCreated(SocketInteraction interaction) {
            switch (interaction) {
                case SocketSlashCommand commandInteraction:
                    await SlashCommandHandler(commandInteraction);
                    break;
                case SocketMessageComponent componentInteraction:
                    await MessageComponentHandler(componentInteraction);
                    break;
            }
        }

        public async Task InitializeAsync() {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage incomingMessage) {
            if (incomingMessage is not SocketUserMessage message) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            if (!message.HasStringPrefix("!", ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result) {
            if (!command.IsSpecified) return;

            if (result.IsSuccess) return;

            await context.Channel.SendMessageAsync($"error: {result}");
        }
        #endregion
    }
}
