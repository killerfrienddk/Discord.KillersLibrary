# Discord.Killer

[![NuGet](https://img.shields.io/badge/nuget-v1.0.0--labs-brightgreen.svg?style=plastic)](https://www.nuget.org/packages/Discord.Killer)
This is an addon for the Discord API Wrapper [Discord.Net-Labs](https://github.com/discord-net-labs/Discord.Net-Labs)

## Installation
Make sure to use the preview version of this package if you are planning to use the preview of Discord.Net.

## Features
 - Creating dynamic embed pages.
   
## Usage
To properly use the features this addon provides you need to add the `EmbedPagesService` to your service provider.

```cs
var provider = new ServiceCollection()
                .AddSingleton<EmbedPagesService>()
                ....
```
Inject the EmbedPagesService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Embed Pages using discord commands structure
```cs
	[Command("Help")]
        [Alias("h")]
        public async Task HelpAsync() {
            List<EmbedBuilder> embedBuilders = new();

            EmbedBuilder embedBuilder = new();
            embedBuilder.WithTitle("Family System");
            embedBuilder.WithDescription(
                "This is the family system.");
            embedBuilder.AddField("Planned Stuff:",
                " - Family tree where it shows the families a bit better.\n " +
                " - Remove Family Member.\n" +
                " - Remove Family Connection.\n" +
                " - Leave Family.\n\n" +
                "Commands to help you:");
            embedBuilder.AddField("!af or !addFamily", "Add a new family.");
            embedBuilders.Add(embedBuilder);

            embedBuilder = new();
            embedBuilder.WithTitle("Animal Searches");
            embedBuilder.WithDescription("This is the animal picture system here is some commands to help you:");
            embedBuilder.AddField("!cat", "Get a cat picture.");
            embedBuilders.Add(embedBuilder);

            await EmbedPagesService.CreateEmbedPages(Context.Client, Context.Message, embedBuilders);
        }
```