# Discord.KillersLibrary

[![NuGet](https://img.shields.io/badge/nuget-v1.0.1--labs-brightgreen.svg?style=plastic)](https://www.nuget.org/packages/Discord.KillersLibrary.Labs)

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
![Embed Pages example](https://i.imgur.com/hUxpVg2.jpg)

Inject the EmbedPagesService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Embed Pages using discord commands structure
```cs
[Command("Help")]
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

### Example: Changing Embed Pages Customization
None of the EmbedPagesStyles has to be set what you see is their default values. 

You can leave out all of them or some of them or change them at will.
```cs
[Command("Help")]
public async Task HelpAsync() {
    EmbedPagesStyles style = new();
    style.FirstLabel = "«";
    style.BackLabel = "‹";
    style.DelEmoji = "🗑";
    style.ForwardLabel = "›";
    style.LastLabel = "»";
    style.Btncolor = ButtonStyle.Success;
    style.Delcolor = ButtonStyle.Danger;
    style.Skipcolor = ButtonStyle.Primary;
    style.FastChangeBtns = false; // Do you want there to be a button that goes directly to either ends?
    style.PageNumbers = true; //Do you want the embed to have page numbers like "Page: 1/4"? Depends on how many pages you have.
    
    await EmbedPagesService.CreateEmbedPages(Context.Client, Context.Message, embedBuilders, style);
}
```
