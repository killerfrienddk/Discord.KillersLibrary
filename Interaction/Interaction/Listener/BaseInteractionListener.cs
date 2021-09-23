using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;

namespace Interaction.Modules.Interaction {

    /*
		Listenes to reactions added to / removed from a message.	
	*/
    public abstract class BaseInteractionListener : IInteractionListener {
        /*
			Lifespan of the listener.
		*/
        public static readonly TimeSpan Default_Lifespan = new TimeSpan(0, 10, 0);
        public static readonly TimeSpan Min_Lifespan = new TimeSpan(0, 0, 10);
        public static readonly TimeSpan Max_Lifespan = new TimeSpan(0, 30, 0);


        /*
			Idling limit of the listener.
		*/
        public static readonly TimeSpan Default_Idle = new TimeSpan(0, 1, 0);
        public static readonly TimeSpan Min_Idle = new TimeSpan(0, 0, 2);
        public static readonly TimeSpan Max_Idle = new TimeSpan(0, 5, 0);


        /*
			Cooldown of the listener.
		*/
        public static readonly TimeSpan Default_Cooldown = new TimeSpan(0, 0, 2);
        public static readonly TimeSpan Min_Cooldown = new TimeSpan(0, 0, 1);
        public static readonly TimeSpan Max_Cooldown = new TimeSpan(0, 1, 0);


        /*
			Longest remaining cooldown it will wait out for the user. 
		*/
        public static readonly TimeSpan Max_Cooldown_Forgiveness = new TimeSpan(0, 0, 1);


        /*
			Standard Emotes
		*/
        public static readonly Emoji Emote_Options = Emojis.Information;
        public static readonly Emoji Emote_Help = Emojis.Question_Mark;


        // ---- //

        private TimeSpan _maxLifespan = Default_Lifespan;
        private TimeSpan _maxIdle = Default_Idle;
        private TimeSpan _cooldown = Default_Cooldown;


        public TimeSpan MaxLifespan {
            get => _maxLifespan;
            set {
                CheckStatus(ListenerStatus.SettingUp);
                if (value < Min_Lifespan)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value cannot be less than {Min_Lifespan}, was {value}!");
                if (value > Max_Lifespan)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value cannot be more than {Max_Lifespan}, was {value}!");
                _maxLifespan = value;
            }
        }
        public TimeSpan MaxIdle {
            get => _maxIdle;
            set {
                CheckStatus(ListenerStatus.SettingUp);
                if (value < Min_Idle)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value cannot be less than {Min_Idle}, was {value}!");
                if (value > Max_Idle)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value cannot be more than {Max_Idle}, was {value}!");
                _maxIdle = value;
            }
        }
        public TimeSpan Cooldown {
            get => _cooldown;
            set {
                CheckStatus(ListenerStatus.SettingUp);
                if (value < Min_Cooldown)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value cannot be less than {Min_Cooldown}, was {value}!");
                if (value > Max_Cooldown)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value cannot be more than {Max_Cooldown}, was {value}!");
                _cooldown = value;
            }
        }



        /*
			The type of listener this is.
		*/
        public ListenerType ListenerType { get; protected set; }

        public DateTimeOffset Timestamp { get; protected set; } = DateTimeOffset.Now;
        public DateTimeOffset LastAction { get; protected set; } = DateTimeOffset.Now;


        public IUserMessage Message { get; protected set; }
        public IMessageChannel Channel { get; protected set; }


        private IUser _target;
        private IUser _owner;

        public IUser Target {
            get => _target;
            protected set {
                _target = value;
                _owner ??= value;

                if (ListenerType == ListenerType.Default)
                    ListenerType = ListenerType.Owned;
            }
        }
        public IUser Owner {
            get => _owner;
            protected set {
                _owner = value;

                if (ListenerType == ListenerType.Default)
                    ListenerType = ListenerType.Hosted;
            }
        }


        public InteractionListenerService Service { get; private set; }
        public ListenerStatus Status { get; private set; } = ListenerStatus.SettingUp;


        public DateTimeOffset Deadline => Timestamp + MaxLifespan;

        private Dictionary<string, IInteractionListenerOption> _options { get; set; } = new Dictionary<string, IInteractionListenerOption>();



        // ---- //


        public Embed CurrentEmbed { get; protected set; }

        public Embed HelpPage { get; private set; }
        public bool IncludeOptionsList { get; private set; } = true;

        public string LastComponentIdUsed { get; private set; }


        // ---- //

        // The task that adds reactions under the message.
        private Task _interactionTask;

        protected readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        protected ComponentBuilder Components { get; set; } = new ComponentBuilder();

        public static ButtonBuilder Button_Help => ButtonBuilder.CreateSecondaryButton("?", "Help");
        protected int RowNr = 0;

        // ---- //

        protected BaseInteractionListener() { }


        //public static ReactionListener Create() { }


        // ---- //


        /*
			Adds a listener to the message with all the navigation actions on it.
		*/
        protected BaseInteractionListener AddOptionInternal(ButtonBuilder button, InteractionHandler handler) {
            //if(Components.ActionRows != null
            //	&& Components.ActionRows.Count > RowNr 
            //	&& Components.ActionRows[RowNr].Components.Count >= 5)
            //	RowNr++;
            //Components.WithButton(button, RowNr);
            //AddOption(new InteractionListenerOption(button.CustomId, handler));
            //return this;


            // Find the first row index that can add this menu.
            var rowNr = Components.FindValidButtonRow(RowNr);

            // If no valid row was found, throw exception.
            if (rowNr == -1)
                throw new InvalidOperationException($"There were no rows left that could add this SelectMenuBuilder.");

            // Update the current rowNr
            RowNr = rowNr;

            // Add the menu to the message component.
            Components.WithButton(button, rowNr);

            // Add the handler for this component
            AddOption(new InteractionListenerOption(button.CustomId, handler));

            return this;
        }

        /*
			Adds a listener to the message with all the navigation actions on it.
		*/
        protected BaseInteractionListener AddOptionInternal(SelectMenuBuilder menu, InteractionHandler handler) {
            //if(Components.ActionRows != null
            //	&& Components.ActionRows.Count > RowNr 
            //	&& Components.ActionRows[RowNr].Components.Count >= 5)
            //	RowNr++;

            // Find the first row index that can add this menu.
            var rowNr = Components.FindValidMenuRow(RowNr);

            // If no valid row was found, throw exception.
            if (rowNr == -1)
                throw new InvalidOperationException($"There were no rows left that could add this SelectMenuBuilder.");

            // Update the current rowNr
            RowNr = rowNr;

            // Add the menu to the message component.
            Components.WithSelectMenu(menu, rowNr);

            // Add the handler for this component
            AddOption(new InteractionListenerOption(menu.CustomId, handler));

            return this;
        }

        // Adds an option to the dictionary
        protected void AddOption(IInteractionListenerOption option) {
            CheckStatus(ListenerStatus.SettingUp);

            _options.Add(option.CustomId, option);
        }


        /*
			Sets the target of listener.
		*/
        public void SetTarget(IUser user) {
            CheckStatus(ListenerStatus.SettingUp);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            Target = user;
        }


        /*
			Sets the owner of listener.
		*/
        public void SetOwner(IUser user) {
            CheckStatus(ListenerStatus.SettingUp);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            Owner = user;
        }


        /*
			Sets the type of listener.
		*/
        public void SetListenerType(ListenerType type) {
            if (type == ListenerType.Default)
                throw new ArgumentException($"Value cannot be set to default!", nameof(type));
            CheckStatus(ListenerStatus.SettingUp);

            ListenerType = type;
        }

        /*
			Adds a help page to the listener.
		*/
        public void AddHelpPage(Embed embed) {
            if (HelpPage != null)
                throw new InvalidOperationException($"{nameof(HelpPage)} is already set.");
            Preconditions.NotNull(embed, nameof(embed));
            CheckStatus(ListenerStatus.SettingUp);

            HelpPage = embed;
        }

        /*
			Adds a page listing all the options to the listener.
		*/
        public void AddOptionsPage(bool enabled = true) {
            CheckStatus(ListenerStatus.SettingUp);

            IncludeOptionsList = enabled;
        }



        /*
			Updates the message.
		*/
        protected async Task<IInteractionResult> UpdateMessage(IInteractionResult res) {
            // Cancel if we shouldn't update
            if (!res.ShouldUpdate)
                return InteractionResult.FromSuccess();


            // If we provided new values
            if (res.Embed != null || res.Components != null) {
                // Update the message
                await Message.ModifyAsync(message => {
                    if (res.Embed != null) message.Embed = res.Embed.Build();
                    if (res.Components != null) message.Components = res.Components.Build();
                });

                // Store the new values
                if (res.Embed == null) CurrentEmbed = res.Embed.Build();
                if (res.Components == null) Components = res.Components;
            }

            // Sets the provided embed as our current one.
            //if(setToCurrentEmbed)
            //	CurrentEmbed = embed;


            return InteractionResult.FromSuccess();
        }

        /*
			Updates the message with the new embed and remove the reaction from the user if marked.
		*/
        protected async Task<IInteractionResult> UpdateMessage(Embed embed, string customId = null, bool setToCurrentEmbed = true) {
            // Replace the embed
            await Message.ModifyAsync(message => message.Embed = embed);

            // Sets the provided embed as our current one.
            if (setToCurrentEmbed)
                CurrentEmbed = embed;

            return InteractionResult.FromSuccess();
        }
        protected Task<IInteractionResult> UpdateMessage(EmbedBuilder embed, string customId = null)
            => UpdateMessage(embed.Build(), customId);

        protected async Task<IInteractionResult> RevertToPreviousEmbed(string customId = null, bool clearLastComponent = true) {
            if (CurrentEmbed == null) {
                return InteractionResult.FromError(InteractionError.BadInput, "Listener has no previous embed to revert to.");
            }

            var res = await UpdateMessage(CurrentEmbed, customId, false);

            // Sets the provided embed as our current one.
            if (clearLastComponent) {
                LastComponentIdUsed = "";
                var neo = InteractionResult.FromResult(res);
                neo.ShouldSaveComponentId = false;
                res = neo;
            }

            return res;
        }



        private IInteractionResult _canRunCommand() {

            // If too fast
            if (LastAction.GetTimeSince() < Cooldown)
                return InteractionResult.FromError(InteractionError.Expired, $"New command issued too fast!");

            // Idle for too long
            if (LastAction.GetTimeSince() > MaxIdle)
                return InteractionResult.FromError(InteractionError.Expired, $"Listener has been idle for too long!", true);

            // Too old
            if (Timestamp.GetTimeSince() > MaxLifespan)
                return InteractionResult.FromError(InteractionError.Expired, $"Listener has lived for too long", true);

            // Update when we last did an action
            LastAction = DateTimeOffset.Now;

            return InteractionResult.FromSuccess();
        }


        /*
			Throws an exception if the listener does not have the desired status.
		*/
        protected void CheckStatus(ListenerStatus status) {
            if (Status != status)
                throw new InvalidOperationException(
                    $"This can only be used while the {nameof(BaseInteractionListener)} is set to {status}, it is currently {Status}!");
        }
        protected void CheckStatus(params ListenerStatus[] statuses) {
            foreach (var status in statuses) {
                if (Status != status)
                    return;
            }
            throw new InvalidOperationException(
                $"This can only be used while the {nameof(BaseInteractionListener)} is set to one of {statuses}, it is currently {Status}!");
        }


        /*
			Builds an embed showing the available options for this listener
		*/
        protected Embed BuildOptionsList() {
            //!> Replace this with the embed generator method when it has been moved down here.
            var embed = new EmbedBuilder();
            var sb = new StringBuilder();

            foreach (var option in _options.Values) {
                sb.AppendLine(option.ToString());
            }

            return embed
                .WithDescription(sb.ToString())
                .Build();
        }


        /*
			Checks if the listener has expired.
		*/
        public bool IsExpired() {
            return (LastAction.GetTimeSince() > MaxIdle) || (Timestamp.GetTimeSince() > MaxLifespan || Status >= ListenerStatus.Completed);
        }


        // Runs when a reaction event has been fired.
        public async Task<IInteractionResult> RunAsync(IInteractionEventParameters par) {
            CheckStatus(ListenerStatus.Started);


            // Prevent more than one task from running here at the same time
            try {
                await _lock.WaitAsync().ConfigureAwait(false);

                // If there is still some cooldown left, but not much, it will wait it out instead.
                var reamainingCooldown = Cooldown - LastAction.GetTimeSince();
                if (TimeSpan.Zero < reamainingCooldown && reamainingCooldown < Max_Cooldown_Forgiveness)
                    await Task.Delay(reamainingCooldown);


                // Check if valid time
                var res = _canRunCommand();
                if (!res.IsSuccess)
                    return res;

                // Check if valid emote
                var customId = par.CustomId;
                if (!_options.TryGetValue(customId, out var option))
                    return InteractionResult.FromError(InteractionError.Unsuccessful, $"Component by id: '{customId}' not listened for.");

                // Run the desired option
                try {
                    res = await option.RunAsync(par);
                }

                // If it throws an uncaught exception:
                catch (Exception e) {
                    // Print it and mark this as a failiure that completes the listener.
                    await SysLog.Instance.Error(GetType(), exception: e);
                    return InteractionResult.FromError(e);
                }

                if (res.IsSuccess && res.ShouldSaveComponentId)
                    LastComponentIdUsed = customId;
                if (res.IsComplete)
                    Status = ListenerStatus.Completed;

                return res;
            }
            // Release the lock
            finally { _lock.Release(); }
        }


        protected void Hook() {

            // Add the help and options buttons if they were enabled 
            if (HelpPage != null) {
                AddOptionInternal(Button_Help, _help);
            }
            //if(IncludeOptionsList)
            //	AddOption(new InteractionListenerOption(Emote_Options, _showOptions, "Show Options"));
        }

        // Connects the listener to a ReactionListenerService.
        public async Task<bool> Connect(InteractionListenerService service) {
            CheckStatus(ListenerStatus.SettingUp);

            Preconditions.NotNull(service, nameof(service));
            Preconditions.NotNull(Message, nameof(Message));

            Service = service;



            if (_options.Count > 0) {
                // Try to listen to the message.
                var res = await service.RegisterNewListner(this);
                Status = ListenerStatus.Started;

                // If we managed to listen to the message: Add the emotes to it.
                if (res) {

                    //_reactionTask = Task.Run( () => AddReactions() );
                    try {

                        //await SysLog.Instance.Debug(GetType(), $"Registring listeners for [ {string.Join(", ", _options.Keys)} ]!");
                        //	
                        //_interactionTask = Task.Run(
                        //	() => Message.AddReactionsAsync(_options.Keys.ToArray(), new RequestOptions {
                        //		CancelToken = _tokenSource.Token,
                        //	})
                        //);


                    } catch (Exception e) {
                        Status = ListenerStatus.Dead;
                        await SysLog.Instance.Error(GetType(), exception: e);
                    }
                }


                return res;
            }
            return false;
        }




        // Kills off the listener.
        public async Task<bool> Kill() {
            CheckStatus(ListenerStatus.Started, ListenerStatus.Completed);

            Status = ListenerStatus.Started;

            try {

                //var builder = Components;
                //builder.DisableComponents();
                var builder = Message.Components.ToBuilder().DisableComponents();

                await Message.ModifyAsync(message => {

                    message.Components = builder.Build();
                });

                return true;
            } catch (Exception e) {
                await SysLog.Instance.Error(GetType(), exception: e);
            }

            return false;
        }


        // ---- //

        private async Task<IInteractionResult> _help(IInteractionEventParameters par) {
            IInteractionResult res;
            if (LastComponentIdUsed == Button_Help.CustomId) {
                res = await RevertToPreviousEmbed(par.CustomId, true);
            } else {
                res = await UpdateMessage(HelpPage, par.CustomId, false);
            }
            return res;
        }
        private async Task<IInteractionResult> _showOptions(IInteractionEventParameters par) {
            if (LastComponentIdUsed == Emote_Options.Name)
                return await RevertToPreviousEmbed(par.CustomId, true);
            else
                return await UpdateMessage(BuildOptionsList(), par.CustomId, false);
        }
    }


    /*
		Extension methods for ReactionListener classes
	*/
    public static class InteractionListenerExtension {

        /*
			Sets the help page of the listener.
		*/
        public static T WithHelpPage<T>(this T listener, Embed embed)
            where T : BaseInteractionListener {
            listener.AddHelpPage(embed);
            return listener;
        }


        /*
			Turns the option page on/off.
		*/
        public static T WithOptionsPage<T>(this T listener, bool enabled = true)
            where T : BaseInteractionListener {
            listener.AddOptionsPage(enabled);
            return listener;
        }



        /*
			Sets the type of listener.
		*/
        public static T WithListenerType<T>(this T listener, ListenerType type)
            where T : BaseInteractionListener {
            listener.SetListenerType(type);
            return listener;
        }

        /*
			Sets the type of listener.
		*/
        public static T WithTarget<T>(this T listener, IUser user)
            where T : BaseInteractionListener {
            listener.SetTarget(user);
            return listener;
        }

        /*
			Sets the type of listener.
		*/
        public static T WithOwner<T>(this T listener, IUser user)
            where T : BaseInteractionListener {
            listener.SetOwner(user);
            return listener;
        }

    }

}