using System.Threading.Tasks;
using System.Linq;
using System;
using Discord;

namespace Interaction.Modules.Interaction {
    public static class PagedInterationListener {
        public static PagedInterationListener<T> Create<T>(Book<T> Book, Func<EmbedBuilder> EmbedGenerator, Action<EmbedBuilder, IPage<T>> PageEmbedder, int pageNr = 1) {
            return new PagedInterationListener<T>(Book, EmbedGenerator, PageEmbedder, pageNr);
        }
    }

    public class PagedInterationListener<T> : BaseInteractionListener {
        public static readonly Emoji Emote_Next = Emojis.Forward;
        public static readonly Emoji Emote_Previous = Emojis.Reverse;
        public static readonly Emoji Emote_First = Emojis.Fast_Reverse;
        public static readonly Emoji Emote_Last = Emojis.Fast_Forward;
        public static readonly Emoji Emote_Stop = Emojis.Stop;
        public static readonly Emoji Emote_Info = Emojis.Information;

        public static ButtonBuilder Button_First => ButtonBuilder.CreatePrimaryButton(null, "First").WithEmote(Emojis.Fast_Reverse);
        public static ButtonBuilder Button_Previous => ButtonBuilder.CreatePrimaryButton(null, "Previous").WithEmote(Emojis.Reverse);
        public static ButtonBuilder Button_Next => ButtonBuilder.CreatePrimaryButton(null, "Next").WithEmote(Emojis.Forward);
        public static ButtonBuilder Button_Last => ButtonBuilder.CreatePrimaryButton(null, "Last").WithEmote(Emojis.Fast_Forward);
        public static ButtonBuilder Button_Random => ButtonBuilder.CreatePrimaryButton(null, "Random").WithEmote(Emojis.Shuffle);

        //Function used to generate new embeds on.
        public Func<EmbedBuilder> EmbedGenerator { get; private set; }

        //Action used to put page into an embed.
        public Action<EmbedBuilder, IPage<T>> PageEmbedder { get; private set; }

        //Book that we wish to navigate through.
        public Book<T> Book { get; private set; }

        //The current page number.
        public int CurrentPage { get; private set; }

        //private Dictionary<string, IInteractionListenerOption> _customOptions { get; set; } = new Dictionary<string, IInteractionListenerOption>();

        public PagedInterationListener(Book<T> Book, Func<EmbedBuilder> EmbedGenerator, Action<EmbedBuilder, IPage<T>> PageEmbedder, int pageNr = 1) {
            if (EmbedGenerator == null) throw new ArgumentNullException(nameof(EmbedGenerator));
            if (PageEmbedder == null) throw new ArgumentNullException(nameof(PageEmbedder));
            if (Book == null) throw new ArgumentNullException(nameof(Book));

            this.EmbedGenerator = EmbedGenerator;
            this.PageEmbedder = PageEmbedder;
            this.Book = Book;
            CurrentPage = Book.GetPageNumber(pageNr - 1) + 1;

            //Add the main buttons.
            AddOptionInternal(Button_First, First);
            AddOptionInternal(Button_Previous, Previous);
            AddOptionInternal(Button_Random, Random);
            AddOptionInternal(Button_Next, Next);
            AddOptionInternal(Button_Last, Last);
        }

        //Create the message and set up the controlls for this navigator.
        public async Task<IUserMessage> Create(
            InteractionListenerService listenerService, IMessageChannel channel,
            IUser target = null, IUser owner = null, MessageSender sender = null
        ) {
            //Check that arguments are valid and we haven't already started this one.
            //if(target == null)
            //throw new ArgumentNullException(nameof(target));
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (listenerService == null) throw new ArgumentNullException(nameof(listenerService));
            if (Channel != null) throw new InvalidOperationException($"This {GetType()} has already been started!");

            //Set the target and channel for this navigator.
            Channel = channel;
            if (target != null) Target = target;
            if (owner != null) Owner = owner;

            Hook();

            //Create embed for the currant page.
            var embed = GetPageEmbed();
            CurrentEmbed = embed.Build();

            //Send the message.

            //If we passed a MessageSender: Use that.
            if (sender != null) Message = await sender(embed: CurrentEmbed, component: Components.Build());
            //Otherwise: sent the message to the given channel.
            else Message = await channel.SendMessageAsync(embed: CurrentEmbed, component: Components.Build());

            //Add a reaction listener to the message.
            AddListener();
            await Connect(listenerService);

            return Message;
        }

        //Wrap the function for a button inside a handler.
        protected InteractionHandler WrapButton(InteractionHandler function) {
            InteractionHandler handler = async parameter => {
                var res = InteractionResult.FromResult(await function(parameter));

                if (res.ShouldUpdate) {
                    res.Embed ??= GetPageEmbed();

                    //Update embed.
                    await UpdateMessage(res.Embed, parameter.CustomId);
                }

                return res;
            };

            return handler;
        }

        //Wrap the function for a togge button inside a handler.
        protected InteractionHandler WrapToggleButton(InteractionHandler function) {
            InteractionHandler handler = async parameter => {
                //Run the function.
                var res = InteractionResult.FromResult(await function(parameter));

                if (res.ShouldUpdate) {
                    res.Embed ??= GetPageEmbed();
                    res.Components ??= Components;

                    //Find the button and toggle it.
                    res.Components.UpdateButton(parameter.CustomId, button => {
                        if (button.Style == ButtonStyle.Danger) button.Style = ButtonStyle.Success;
                        else button.Style = ButtonStyle.Danger;
                    });

                    //Update embed.
                    await UpdateMessage(res);
                }

                return res;
            };

            return handler;
        }

        //Adds a toggle button to the message.
        public PagedInterationListener<T> WithToggleButton(ButtonBuilder button, InteractionHandler function) {
            //Wrap the function.
            InteractionHandler handler = WrapToggleButton(function);

            AddOptionInternal(button, handler);

            return this;
        }

        //Adds a button to the message.
        public PagedInterationListener<T> WithButton(ButtonBuilder button, InteractionHandler function) {
            //Wrap the function.
            InteractionHandler handler = WrapButton(function);

            AddOptionInternal(button, handler);

            return this;
        }

        //Generates an embed for the desired page.
        protected EmbedBuilder GetEmbed() {
            return EmbedGenerator();
        }

        //Adds a listener to the message with all the navigation actions on it.
        private void AddListener() {
            //AddOption(new InteractionListenerOption(Button_First.CustomId, 		First));
            //AddOption(new InteractionListenerOption(Button_Previous.CustomId, 	Previous));
            //AddOption(new InteractionListenerOption(Button_Next.CustomId, 		Next));
            //AddOption(new InteractionListenerOption(Button_Last.CustomId, 		Last));

            //foreach(var pair in _customOptions) {
            //	AddOption(pair.Value);
            //}
        }

        //Generates an embed for the desired page.
        protected EmbedBuilder GetPageEmbed() {
            //Create a new embed.
            var embed = EmbedGenerator();

            //Embed the page to it.
            PageEmbedder(embed, Book.GetPage(CurrentPage - 1));

            //Put what page we are on in the footer.
            embed.WithFooter($"Page: {CurrentPage}/{Book.PageCount}");

            return embed;
        }

        #region Actions
        protected async Task<IInteractionResult> Next(IInteractionEventParameters par) {
            //If on last page, cancel.
            if (CurrentPage == Book.PageCount) return InteractionResult.FromError(InteractionError.Unsuccessful, "Already on the last page.");

            //Increment pages.
            CurrentPage++;

            //Update embed and remove emote.
            return await UpdateMessage(GetPageEmbed(), par.CustomId);
        }

        protected async Task<IInteractionResult> Previous(IInteractionEventParameters par) {
            //If on first page, cancel.
            if (CurrentPage == 1) return InteractionResult.FromError(InteractionError.Unsuccessful, "Already on the first page.");

            //Increment pages.
            CurrentPage--;

            //Update embed and remove emote.
            return await UpdateMessage(GetPageEmbed(), par.CustomId);
        }

        protected async Task<IInteractionResult> Last(IInteractionEventParameters par) {
            //If on last page, cancel.
            if (CurrentPage == Book.PageCount) return InteractionResult.FromError(InteractionError.Unsuccessful, "Already on the last page.");

            //Increment pages.
            CurrentPage = Book.PageCount;

            //Update embed and remove emote.
            return await UpdateMessage(GetPageEmbed(), par.CustomId);
        }

        protected async Task<IInteractionResult> First(IInteractionEventParameters par) {
            //If on last page, cancel.
            if (CurrentPage == 1) return InteractionResult.FromError(InteractionError.Unsuccessful, "Already on the first page.");

            //Increment pages.
            CurrentPage = 1;

            //Update embed and remove emote.
            return await UpdateMessage(GetPageEmbed(), par.CustomId);
        }

        protected async Task<IInteractionResult> Random(IInteractionEventParameters par) {
            //Increment pages.
            CurrentPage = new Random().Next(Book.PageCount) + 1;

            //Update embed and remove emote.
            return await UpdateMessage(GetPageEmbed(), par.CustomId);
        }
        #endregion
    }
}