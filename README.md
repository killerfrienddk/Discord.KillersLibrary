# Discord.KillersLibrary

[![NuGet](https://buildstats.info/nuget/Discord.KillersLibrary)](https://www.nuget.org/packages/Discord.KillersLibrary)
[![Discord](https://discord.com/api/guilds/890629777818542092/widget.png)](https://discord.gg/2rFB54xQs7)
[![NuGet](https://buildstats.info/nuget/Discord.KillersLibrary.Labs)](https://www.nuget.org/packages/Discord.KillersLibrary.Labs) - Labs version

This is an addon for the Discord API Wrapper [Discord.Net](https://github.com/discord-net/Discord.Net).

## Features
 - [Creating dynamic embed pages](https://github.com/killerfrienddk/Discord.KillersLibrary#embed-pages).
 - [Creating dynamic multi buttons](https://github.com/killerfrienddk/Discord.KillersLibrary#multi-buttons).
 - [Creating Multiple Messages Multi Buttons](https://github.com/killerfrienddk/Discord.KillersLibrary#multiple-messages-multi-buttons).
 - [Multi buttons select](https://github.com/killerfrienddk/Discord.KillersLibrary#multi-buttons-select).
 - [Removing all multi buttons messages and the select message.](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/README.md#example-removing-all-multi-buttons-messages-and-the-select-message) .
 - [There is some Methods that will return either a DiscordID From either slash or normal commands](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/Killer/Services/CommonService.cs).
 - [Same thing for GuildID](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/Killer/Services/CommonService.cs).
 - [It does also contain FileUpload and SendMessages that both work with the slash and normal commands](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/Killer/Services/CommonService.cs).
 - [Discord.net Preconditions](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/Killer/Utilities/Preconditions.cs).
 - [Markdown Utilities](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/Killer/Utilities/MarkdownUtilities.cs).
 - [String Utilities](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/Killer/Utilities/StringUtilities.cs).
 - More is to be added in the future.

## Wiki
[Wiki on github](https://github.com/killerfrienddk/Discord.KillersLibrary/wiki).

## Other Libraries
[Database Connection for mysql](https://github.com/killerfrienddk/DatabaseConnection).

## Usage
To properly use the features this addon provides you need to add the `EmbedPagesService` or `MultiButtonsService` to your service provider depending on which part you want.

```csharp
var provider = new ServiceCollection()
    .AddSingleton<EmbedPagesService>() // For embedding pages
    .AddSingleton<MultiButtonsService>(); // For multi buttons
    ....
```
### Dependency Injection in commands.
```csharp
public EmbedPagesService EmbedPagesService { get; set; }
public MultiButtonsService MultiButtonsService { get; set; }
```

### Dependency Injection using ctor.
```csharp
private readonly EmbedPagesService _embedPagesService;
private readonly MultiButtonsService _multiButtonsService;

public CTOR(EmbedPagesService embedPagesService, MultiButtonsService multiButtonsService) {
    _embedPagesService = embedPagesService;
    _multiButtonsService = multiButtonsService;
}
```

## Embed Pages
![Embed Pages example](https://i.imgur.com/hUxpVg2.jpg)

[Inject](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/README.md#dependency-injection-in-commands) the EmbedPagesService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Embed Pages using discord commands structure or slash commands
```csharp
[Command("Help")] // Remove this for slash commands
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
    embedBuilder.WithColorCode(ColorCodes.Aqua); // Use the colorCodes to find your specific color of what you want to use.
    embedBuilders.Add(embedBuilder);

    embedBuilder = new();
    embedBuilder.WithTitle("Animal Searches");
    embedBuilder.WithDescription("This is the animal picture system here is some commands to help you:");
    embedBuilder.AddField("!cat", "Get a cat picture.");
    embedBuilder.WithColorCode(ColorCodes.White); // Use the colorCodes to find your specific color of what you want to use.
    embedBuilders.Add(embedBuilder);
    
    await _embedPagesService.CreateEmbedPages(client, embedBuilders, context);
    // await _embedPagesService.CreateEmbedPages(client, embedBuilders, command: command); //Or slashcommands
}
```

### Example: Changing Embed Pages Customization
None of the EmbedPagesStyles has to be set what you see is their default values. 

You can leave out all of them or some of them or change them at will.
```csharp
[Command("Help")] // Remove this for slash commands
public async Task HelpAsync() {
    EmbedPagesStyles style = new();
    style.FirstLabel = "«";
    style.BackLabel = "‹";
    style.DeletionEmoji = "🗑";
    style.ForwardLabel = "›";
    style.LastLabel = "»";
    style.DeletionMessage = "Embed page has been deleted";
    style.BtnColor = ButtonStyle.Success;
    style.DeletionBtnColor = ButtonStyle.Danger;
    style.SkipBtnColor = ButtonStyle.Primary;
    style.FastChangeBtns = false; // Do you want there to be a button that goes directly to either ends?
    style.PageNumbers = true; //Do you want the embed to have page numbers like "Page: 1/4"? Depends on how many pages you have.
    style.RemoveDeleteBtn = false; // Set this to true if you want to remove the Delete Button. Keep as false, or remove this line to keep the Delete Button.
    
    await _embedPagesService.CreateEmbedPages(client, embedBuilders, context, style: style);
    // await _embedPagesService.CreateEmbedPages(client, embedBuilders, command: command, style: style); //Or slashcommands
}
```

## Multi Buttons
![Multi Buttons example](https://i.imgur.com/9tXMrmS.jpg)

[Inject](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/README.md#dependency-injection-in-commands) the MultiButtonsService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Multi Buttons
You can do as i have done for making buttons that splits up people in your server by 25 pr button.
Or just a long list of strings.

In order to get a list of users you have to activate the "Privileged Gateway Intents" those being "PRESENCE INTENT" and "SERVER MEMBERS INTENT" they can be set [here](https://discord.com/developers/applications) by choosing your bot and going to the Bots tab. 
Remember if you your bot is in 100 or more servers then it needs to get verification and whitelisting from discord for the intents to work. [Read more here](https://support.discord.com/hc/en-us/articles/360040720412).
```csharp
public async Task CreateChooseChildButtons(SocketMessageComponent interaction) {
    List<RestGuildUser> users = await _userService.GetSortedUserListAsync(((SocketGuildUser)interaction.User).Guild);

    List<string> titles = new();
    foreach (RestGuildUser user in users) titles.Add(user.Nickname ?? user.Username);

    var builder = _multiButtonsService.CreateMultiButtons(titles);

    await interaction.FollowupAsync("Choose Person", component: builder.Build());
}
```

### Example: Changing Multi Buttons Customization
None of the MultiButtonsStyles has to be set what you see is their default values. 

You can leave out all of them or some of them or change them at will.
```csharp
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

    await interaction.FollowupAsync("Choose Person", component: builder.Build());
}
```

## Multipule Messages Multi Buttons
[Inject](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/README.md#dependency-injection-in-commands) the MultiButtonsService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Multiple Messages Multi Buttons
You can do as i have done for making buttons that splits up people in your server by 25 pr button.
Or just a long list of strings.

In order to get a list of users you have to activate the "Privileged Gateway Intents" those being "PRESENCE INTENT" and "SERVER MEMBERS INTENT" they can be set [here](https://discord.com/developers/applications) by choosing your bot and going to the Bots tab. 
Remember if you your bot is in 100 or more servers then it needs to get verification and whitelisting from discord for the intents to work. [Read more here](https://support.discord.com/hc/en-us/articles/360040720412).
```csharp
public async Task CreateChooseChildButtons(SocketMessageComponent interaction) {
    List<RestGuildUser> users = await _userService.GetSortedUserListAsync(((SocketGuildUser)interaction.User).Guild);

    List<string> titles = new();
    foreach (RestGuildUser user in users) titles.Add(user.Nickname ?? user.Username);

    List<ComponentBuilder> builders = _multiButtonsService.CreateMultipleMultiButtons(titles);

    foreach (ComponentBuilder builder in builders) {
        await interaction.FollowupAsync("Choose Person", component: builder.Build());
    }
}
```

After you use this you have to use the other part which is the selector: [Link to Multi Buttons Select](https://github.com/killerfrienddk/Discord.KillersLibrary#multi-buttons-select).

### Example: Changing Multiple Messages Multi Buttons Customization
None of the MultiButtonsStyles has to be set what you see is their default values. 

You can leave out all of them or some of them or change them at will.
```csharp
public async Task CreateChooseChildButtons(SocketMessageComponent interaction) {
    List<RestGuildUser> users = await _userService.GetSortedUserListAsync(((SocketGuildUser)interaction.User).Guild);

    List<string> titles = new();
    foreach (RestGuildUser user in users) titles.Add(user.Nickname ?? user.Username);

    MultipleMultiButtonsStyles multipleMultiButtonsStyles = new() {
        CustomID = "multiButtons",
        ButtonStyle = ButtonStyle.Success,
        UpperCaseLetters = true,
        OrderByTitle = true,
        DoNotAddLastButton = false,
        MultiButtonsRows = MultiButtonsRows.Five
    };

    List<ComponentBuilder> builders = _multiButtonsService.CreateMultiButtons(titles, MultiButtonsRows.Five, multipleMultiButtonsStyles);
    // List<ComponentBuilder> builders = _multiButtonsService.CreateMultipleMultiButtons(titles, styles: multipleMultiButtonsStyles);

    foreach (ComponentBuilder builder in builders) {
        await interaction.FollowupAsync("Choose Person", component: builder.Build());
    }
}
```


## Multi Buttons Select
![MultiButtons Select example](https://i.imgur.com/7MM1il5.jpg)

[Inject](https://github.com/killerfrienddk/Discord.KillersLibrary/blob/main/README.md#dependency-injection-in-commands) the MultiButtonsService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Multi Buttons Select
In order to get a list of users you have to activate the "Privileged Gateway Intents" those being "PRESENCE INTENT" and "SERVER MEMBERS INTENT" they can be set [here](https://discord.com/developers/applications) by choosing your bot and going to the Bots tab. 
Remember if you your bot is in 100 or more servers then it needs to get verification and whitelisting from discord for the intents to work. [Read more here](https://support.discord.com/hc/en-us/articles/360040720412).

Place this in side of your Button Handler and set the "multiButtons" to what your customId is.
```csharp
private async Task ButtonHandler(SocketMessageComponent interaction) {
    if (Regex.IsMatch(customId, "multiButtons[0-9]+")) await _buttonCommands.ChooseChildNameRange(interaction);
}
```
You need to give it the full list and it will figure out the rest.
Again you can use it as i have with users or your own list.
```csharp
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

    var builder = await _multiButtonsService.CreateSelectForMultiButtonsAsync(interaction, multiButtons);

    await interaction.FollowupAsync("Choose Person", component: builder.Build());
}
```

### Example: Changing Multi Buttons Selection Customization
None of the MultiButtonsStyles has to be set what you see is their default values. 

You can leave out all of them or some of them or change them at will.
```csharp
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

    await interaction.FollowupAsync("Choose Person", component: builder.Build());
}
```

### Example: Removing all multi buttons messages and the select message
```csharp
public async Task OnSelectionChooseRange(SocketMessageComponent interaction) {
    await _multiButtonsService.RemoveMultiButtonsAndSelectAsync(interaction);
}
```
