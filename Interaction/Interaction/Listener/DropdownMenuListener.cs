using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Discord;

namespace Interaction.Modules.Interaction {
    //public static class DropdownMenuListener {
    //	public static DropdownMenuListener<T> Create<T>(Book<T> Book, Func<EmbedBuilder> EmbedGenerator, Action<EmbedBuilder, IPage<T>> PageEmbedder, int pageNr = 1) {
    //		return new DropdownMenuListener<T>(Book, EmbedGenerator, PageEmbedder, pageNr);
    //	}
    //}

    //Delegate that produces an embed for a given menu choice.
    public delegate Task<EmbedBuilder> MenuSelectEmbedGenerator(IInteractionEventParameters parameters);

    //A listener that swiches page based on the selected option.
    public class DropdownMenuListener : BaseInteractionListener {
        public const string MENU_CUSTOM_ID = "main_menu";

        //The dropdown menu.
        protected SelectMenuBuilder Menu { get; private set; } = new SelectMenuBuilder {
            CustomId = MENU_CUSTOM_ID,
            Options = new List<SelectMenuOptionBuilder>(),
        };

        //Dictionary binding menu choices to embeds.
        protected Dictionary<string, InteractionHandler> MenuOptions { get; private set; }
            = new Dictionary<string, InteractionHandler>();

        public DropdownMenuListener() { }

        public DropdownMenuListener(EmbedBuilder embed) : this() {
            CurrentEmbed = embed.Build();
        }

        //Create the message and set up the controlls for this navigator.
        public async Task<IUserMessage> Create(
            InteractionListenerService listenerService, IMessageChannel channel,
            EmbedBuilder embed = null, IUser target = null, IUser owner = null, MessageSender sender = null
        ) {
            //Check that arguments are valid and we haven't already started this one.
            //if(target == null)
            //throw new ArgumentNullException(nameof(target));
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (listenerService == null) throw new ArgumentNullException(nameof(listenerService));
            if (Channel != null) throw new InvalidOperationException($"This {GetType()} has already been started!");
            if (MenuOptions.Count == 0) throw new InvalidOperationException($"Listener must have at least 1 option!");

            //Add new embed if it's provided.
            if (embed != null) CurrentEmbed = embed.Build();

            if (CurrentEmbed == null) throw new InvalidOperationException($"Needs an embed to start with.");

            //Set the target and channel for this navigator.
            Channel = channel;
            if (target != null)
                Target = target;
            if (owner != null)
                Owner = owner;

            //Add the selection menu to the listener.
            AddOptionInternal(Menu, OnMenuSelect);

            Hook();

            //Send the message.
            //If we passed a MessageSender: Use that.
            if (sender != null) Message = await sender(embed: CurrentEmbed, component: Components.Build());
            //Otherwise: sent the message to the given channel.
            else Message = await channel.SendMessageAsync(embed: CurrentEmbed, component: Components.Build());

            await Connect(listenerService);

            return Message;
        }

        //Adds an option to the menu.
        public DropdownMenuListener WithOption(SelectMenuOptionBuilder option, InteractionHandler function) {
            if (option == null) throw new ArgumentNullException(nameof(option));
            if (function == null) throw new ArgumentNullException(nameof(function));
            if (option.Value == null) throw new ArgumentException($"Option must have a value.", nameof(option));
            if (string.IsNullOrEmpty(option.Label) && option.Emote == null)
                throw new ArgumentException($"Option must have a label or an emote.", nameof(option));
            if (Menu.Options.Count > SelectMenuBuilder.MaxOptionCount)
                throw new InvalidOperationException($"Menu already has {SelectMenuBuilder.MaxOptionCount} options.");

            //foreach(var val in Menu.Options.Select(option => option.Value))
            //	if(val == option.Value)
            //		throw new ArgumentException($"Option value must be unique, option of value {val} already exists.", nameof(option));

            //Add the function to the dictionary.
            if (!MenuOptions.TryAdd(option.Value, function))
                throw new InvalidOperationException($"That value is already in use.");

            //Add the option to the menu.
            Menu.Options.Add(option);

            return this;
        }

        #region Actions
        //Handles what to do if an option was selected.
        protected async Task<IInteractionResult> OnMenuSelect(IInteractionEventParameters par) {
            var res = InteractionResult.FromResult(await MenuOptions[par.Values.First()](par));

            if (res.ShouldUpdate && res.Embed != null) await UpdateMessage(res);

            return res;
        }
        #endregion
    }
}