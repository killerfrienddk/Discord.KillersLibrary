using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using KillersLibraryTestBot.Services.Commands;
using KillersLibraryTestBot.Services;
using KillersLibrary.Services;

namespace KillersLibraryTestBot {
    class Program {
        static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
            await client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log) {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices() {
            ServiceCollection serviceCollection = new();

            serviceCollection // Standard
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
               .AddSingleton<SelectCommandsService>()

                ;

            serviceCollection // Own
                .AddSingleton<UserService>()
                .AddSingleton<CommonFunctionService>()
                .AddSingleton<ButtonCommandsService>()
                .AddSingleton<CommandsService>()

                ;

            serviceCollection // Killers Lib
                .AddSingleton<EmbedPagesService>()
                .AddSingleton<MultiButtonsService>()

                ;

            return serviceCollection.BuildServiceProvider();
        }
    }
}