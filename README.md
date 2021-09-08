# Discord.KillersLibrary

[![NuGet](https://img.shields.io/badge/nuget-v1.0.4--labs-brightgreen.svg?style=plastic)](https://www.nuget.org/packages/Discord.KillersLibrary.Labs)

This is an addon for the Discord API Wrapper [Discord.Net-Labs](https://github.com/discord-net-labs/Discord.Net-Labs)

## Installation
Make sure to use the preview version of this package if you are planning to use the preview of Discord.Net.

## Features
 - Creating dynamic embed pages.
 - Creating dynamic Multi buttons and after that selection.
   
## Usage
To properly use the features this addon provides you need to add the `EmbedPagesService` to your service provider.

```cs
var provider = new ServiceCollection()
                .AddSingleton<EmbedPagesService>() // For embedding pages
                .AddSingleton<MultiButtonsService>() // For multi buttons
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
    style.DeletionMessage = "Embed page has been deleted";
    style.Btncolor = ButtonStyle.Success;
    style.Delcolor = ButtonStyle.Danger;
    style.Skipcolor = ButtonStyle.Primary;
    style.FastChangeBtns = false; // Do you want there to be a button that goes directly to either ends?
    style.PageNumbers = true; //Do you want the embed to have page numbers like "Page: 1/4"? Depends on how many pages you have.
    
    await EmbedPagesService.CreateEmbedPages(Context.Client, Context.Message, embedBuilders, style);
}
```

![Multi Buttons example](https://i.imgur.com/9tXMrmS.jpg)

Inject the MultiButtonsService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Multi Buttons
You can do as i have done for making buttons that splits up people in your server by 25 pr button.
Or just a long list of strings.
```cs
public async Task CreateChooseChildButtons(SocketMessageComponent interaction) {
    List<RestGuildUser> users = await _userService.GetSortedUserListAsync(((SocketGuildUser)interaction.User).Guild);

    List<string> titles = new();
    foreach (RestGuildUser user in users) titles.Add(user.Nickname ?? user.Username);

    var builder = _multiButtonsService.CreateMultiButtons(titles, multiButtonsStyles);
    builder.WithButton(_commonService.MakeGoBackButton());

    await interaction.FollowupAsync(text: "Choose Person", component: builder.Build());
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

    await interaction.FollowupAsync(text: "Choose Person", component: builder.Build());
}
```

![MultiButtons Select example](https://i.imgur.com/7MM1il5.jpg)

Inject the MultiButtonsService into your Module using DI instead. (Constructor / Public Property Injection).

### Example: Creating Multi Buttons Select
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

    await interaction.FollowupAsync(text: "Choose Person", component: builder.Build());
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

    await interaction.FollowupAsync(text: "Choose Person", component: builder.Build());
}
```
