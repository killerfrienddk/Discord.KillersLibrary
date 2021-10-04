using System.Threading.Tasks;
using System;
using Discord;

namespace Interaction.TestingGrounds.Listener {
    public class CustomInteractionListener : BaseInteractionListener {
        public EmbedBuilder Embed { get; private set; }
        public Func<EmbedBuilder> EmbedGenerator { get; private set; }
        //protected ComponentBuilder Components = new ComponentBuilder();

        public CustomInteractionListener(Func<EmbedBuilder> EmbedGenerator) {
            if (EmbedGenerator == null) throw new ArgumentNullException(nameof(EmbedGenerator));

            this.EmbedGenerator = EmbedGenerator;
        }

        //Create the message and set up the controlls for this navigator.
        public async Task<IUserMessage> Create(
            InteractionListenerService listenerService, IMessageChannel channel = null,
            EmbedBuilder embed = null, IUser target = null, IUser owner = null,
            MessageSender sender = null
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
            Embed = embed ?? GetEmbed();
            CurrentEmbed = embed.Build();

            //Send the message.
            //If we passed a MessageSender: Use that.
            if (sender != null) Message = await sender(embed: CurrentEmbed, component: Components.Build());
            //Otherwise: sent the message to the given channel.
            else Message = await channel.SendMessageAsync(embed: CurrentEmbed, component: Components.Build());

            //Add a reaction listener to the message.
            await Connect(listenerService);

            return Message;
        }

        //Wrap the function inside a handler.
        protected InteractionHandler Wrap(InteractionHandlerWithEmbedder function) {
            InteractionHandler handler = async parameter => {
                var embed = GetEmbed();
                var res = InteractionResult.FromResult(await function(parameter, embed));

                if (res.ShouldUpdate) {
                    res.Embed ??= embed;
                    await UpdateMessage(res.Embed, parameter.CustomId);
                }

                return res;
            };

            return handler;
        }

        //Adds a listener to the message with all the navigation actions on it.
        public CustomInteractionListener AddOption(ButtonBuilder button, InteractionHandlerWithEmbedder function) {
            //Wrap the function.
            InteractionHandler handler = Wrap(function);

            AddOptionInternal(button, handler);

            return this;
        }

        //Adds a listener to the message with all the navigation actions on it.
        public CustomInteractionListener AddOption(SelectMenuBuilder menu, InteractionHandlerWithEmbedder function) {
            //Wrap the function.
            InteractionHandler handler = Wrap(function);

            AddOptionInternal(menu, handler);

            return this;
        }

        //Sets the row number to add new buttons to.
        public CustomInteractionListener WithRowNr(int neo) {
            RowNr = neo;
            return this;
        }

        //Generates an embed for the desired page.
        protected EmbedBuilder GetEmbed() {
            return EmbedGenerator();
        }
    }
}