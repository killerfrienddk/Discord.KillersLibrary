# Discord.KillersLibrary

[![NuGet](https://img.shields.io/badge/nuget-v1.0.9--labs-brightgreen.svg?style=plastic)](https://www.nuget.org/packages/Discord.KillersLibrary.Labs)

This is an addon for the Discord API Wrapper [Discord.Net-Labs](https://github.com/discord-net-labs/Discord.Net-Labs).

## Installation
Make sure to use the preview version of this package if you are planning to use the preview of Discord.Net.Labs.

## Features
 - [Creating dynamic embed pages](https://github.com/killerfrienddk/Discord.KillersLibrary.Labs#embed-pages).
 - [Creating dynamic multi buttons](https://github.com/killerfrienddk/Discord.KillersLibrary.Labs#multi-buttons).
 - [Multi buttons select](https://github.com/killerfrienddk/Discord.KillersLibrary.Labs#multi-buttons-select). - [There is some Methods that will return either a DiscordID From either slash or normal commands.](https://github.com/killerfrienddk/Discord.KillersLibrary.Labs/blob/main/Killer/CommonService.cs)
 - [Same thing for GuildID](https://github.com/killerfrienddk/Discord.KillersLibrary.Labs/blob/main/Killer/CommonService.cs)
 - [It does also contain FileUpload and SendMessages that both work with the slash and normal commands.](https://github.com/killerfrienddk/Discord.KillersLibrary.Labs/blob/main/Killer/CommonService.cs)
 - More is to be added in the future.

## Wiki
[Wiki on github](https://github.com/killerfrienddk/Discord.KillersLibrary.Labs/wiki).
## Usage
To properly use the features this addon provides you need to add the `EmbedPagesService` or `MultiButtonsService` to your service provider depending on which part you want.

```cs
var provider = new ServiceCollection()
    .AddSingleton<EmbedPagesService>() // For embedding pages
    .AddSingleton<MultiButtonsService>() // For multi buttons
    ....
```
### Dependency Injection in commands.
```cs
public EmbedPagesService EmbedPagesService { get; set; }
public MultiButtonsService MultiButtonsService { get; set; }
```

### Dependency Injection using ctor.
```cs
private readonly EmbedPagesService _embedPagesService;
private readonly MultiButtonsService _multiButtonsService;

public CTOR(EmbedPagesService embedPagesService, MultiButtonsService multiButtonsService) {
    _embedPagesService = embedPagesService;
    _multiButtonsService = multiButtonsService;
}
```

## Embed Pages
![Embed Pages example](https://i.imgur.com/hUxpVg2.jpg)

Inject the EmbedPagesService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Embed Pages using discord commands structure or slash commands
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
    
    await _embedPagesService.CreateEmbedPages(client, embedBuilders, context);
    // await _embedPagesService.CreateEmbedPages(client, embedBuilders, command: command); //Or slashcommands
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
    style.DeletionMessage = "Embed page has been deleted";
    style.Btncolor = ButtonStyle.Success;
    style.Delcolor = ButtonStyle.Danger;
    style.Skipcolor = ButtonStyle.Primary;
    style.FastChangeBtns = false; // Do you want there to be a button that goes directly to either ends?
    style.PageNumbers = true; //Do you want the embed to have page numbers like "Page: 1/4"? Depends on how many pages you have.
    
    await _embedPagesService.CreateEmbedPages(client, embedBuilders, context, style: style);
    // await _embedPagesService.CreateEmbedPages(client, embedBuilders, command: command, style: style); //Or slashcommands
}
```

## Multi Buttons
![Multi Buttons example](https://i.imgur.com/9tXMrmS.jpg)

Inject the MultiButtonsService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Multi Buttons
You can do as i have done for making buttons that splits up people in your server by 25 pr button.
Or just a long list of strings.

In order to get a list of users you have to activate the "Privileged Gateway Intents" those being "PRESENCE INTENT" and "SERVER MEMBERS INTENT" they can be set [here](https://discord.com/developers/applications) by choosing your bot and going to the Bots tab. 
Remember if you your bot is in 100 or more servers then it needs to get verification and whitelisting from discord for the intents to work. [Read more here](https://support.discord.com/hc/en-us/articles/360040720412).
```cs
public async Task CreateChooseChildButtons(SocketMessageComponent interaction) {
    List<RestGuildUser> users = await _userService.GetSortedUserListAsync(((SocketGuildUser)interaction.User).Guild);

    List<string> titles = new();
    foreach (RestGuildUser user in users) titles.Add(user.Nickname ?? user.Username);

    var builder = _multiButtonsService.CreateMultiButtons(titles, multiButtonsStyles);
    builder.WithButton(_commonService.MakeGoBackButton());

    await interaction.FollowupAsync("Choose Person", component: builder.Build());
}
```

### Example: Changing Multi Buttons Customization
None of the MultiButtonsStyles has to be set what you see is their default values. 

You can leave out all of them or some of them or change them at will.
```cs 
public async Task CreateChooseChildButtons(SocketMessageComponent interaction) {
    List<RestGuildUser> users = await _userService.GetSortedUserListAsync(((SocketGuildUser)interaction.User).Guild);

    List<string> titles = new();
    foreach (RestGuildUser user in users) titles.Add(user.Nickname ?? user.Username);

    MultiButtonsStyles multiButtonsStyles = new() {
        CustomID = "multiButtons",
        ButtonStyle = ButtonStyle.Success,
        UpperCaseLetters = true,
        OrderByTitle = true
    };

    var builder = _multiButtonsService.CreateMultiButtons(titles, multiButtonsStyles);
    builder.WithButton(_commonService.MakeGoBackButton());

    await interaction.FollowupAsync("Choose Person", component: builder.Build());
}
```

## Multi Buttons Select
![MultiButtons Select example](https://i.imgur.com/7MM1il5.jpg)

Inject the MultiButtonsService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Multi Buttons Select
In order to get a list of users you have to activate the "Privileged Gateway Intents" those being "PRESENCE INTENT" and "SERVER MEMBERS INTENT" they can be set [here](https://discord.com/developers/applications) by choosing your bot and going to the Bots tab. 
Remember if you your bot is in 100 or more servers then it needs to get verification and whitelisting from discord for the intents to work. [Read more here](https://support.discord.com/hc/en-us/articles/360040720412).

Place this in side of your Button Handler and set the "multiButtons" to what your customId is.
```cs
private async Task ButtonHandler(SocketMessageComponent interaction) {
    if (Regex.IsMatch(customId, "multiButtons[0-9]+")) await _buttonCommands.ChooseChildNameRange(interaction);
}
```
You need to give it the full list and it will figure out the rest.
Again you can use it as i have with users or your own list.
```cs
public async Task ChooseChildNameRange(SocketMessageComponent interaction) {
    List<MultiButton> multiButtons = new();
    List<RestGuildUser> users = await _userService.GetSortedUserListAsync(((SocketGuildUser)interaction.User).Guild);
    foreach (RestGuildUser user in users) {
        MultiButton multiButton = new() {
            Title = user.Nickname ?? user.Username,
            Value = user.Id.ToString()
        };

        multiButtons.Add(multiButton);
    }

    var builder = _multiButtonsService.CreateSelectForMultiButtons(interaction, multiButtons, selectForMultiButtonsStyles);
    builder.WithButton(_commonService.MakeGoBackButton());

    await interaction.FollowupAsync("Choose Person", component: builder.Build());
}
```

### Example: Changing Multi Buttons Selection Customization
None of the MultiButtonsStyles has to be set what you see is their default values. 

You can leave out all of them or some of them or change them at will.
```cs
public async Task ChooseChildNameRange(SocketMessageComponent interaction) {
    List<MultiButton> multiButtons = new();
    List<RestGuildUser> users = await _userService.GetSortedUserListAsync(((SocketGuildUser)interaction.User).Guild);
    foreach (RestGuildUser user in users) {
        MultiButton multiButton = new() {
            Title = user.Nickname ?? user.Username,
            Value = user.Id.ToString()
        };

        multiButtons.Add(multiButton);
    }

    SelectForMultiButtonsStyles selectForMultiButtonsStyles = new() {
        CustomID = "chooseRange",
        Placeholder = "Select Item",
        RagedLettersOnEndOfPlaceholder = true,
        OrderByTitle = true
    };

    var builder = _multiButtonsService.CreateSelectForMultiButtons(interaction, multiButtons, selectForMultiButtonsStyles);
    builder.WithButton(_commonService.MakeGoBackButton());

    await interaction.FollowupAsync("Choose Person", component: builder.Build());
}
```
