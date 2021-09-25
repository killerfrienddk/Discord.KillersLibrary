using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using System.Text;
using System;
using Discord;
using Timer = System.Timers.Timer;

namespace Interaction.Modules.Interaction {
    //Service that manages what to do when interactions occur.
    public class InteractionListenerService : IInteractionListenerService {
        const int SOFT_CAP = 10;    //Log warns when soft cap is exceeded.
        const int HARD_CAP = 20;    //Throw errors and refuse to add more listners when hard cap is exceeded.

        static readonly TimeSpan MIN_WAIT = new TimeSpan(0, 0, 10);
        static readonly TimeSpan MAX_WAIT = new TimeSpan(0, 2, 0);
        static readonly TimeSpan DEFAULT_WAIT = new TimeSpan(0, 0, 30);

        //Number of milliseconds between each time we prune inactive listeners.
        static readonly int PRUNE_INTERVAL = 5_000;

        //The dictionary storing listeners by the target message.
        protected Dictionary<ulong, IInteractionListener> _messageDict = new Dictionary<ulong, IInteractionListener>();


        //The dictionary storing listeners by target user.
        protected Dictionary<ulong, IInteractionListener> _userDict = new Dictionary<ulong, IInteractionListener>();

        //The dictionary storing listeners by hosting user.
        protected Dictionary<ulong, IInteractionListener> _hostDict = new Dictionary<ulong, IInteractionListener>();

        public int Count => _messageDict.Count;

        private readonly IServiceProvider _services;
        private readonly ContextualizerService _contextualizerService;
        private readonly SysLog _logger;

        private Timer _timer;

        //The dictionary storing listeners.
        public IReadOnlyDictionary<ulong, IInteractionListener> Dict => _messageDict;

        //The messages currently listened to.
        public IReadOnlyCollection<ulong> Messages => _messageDict.Keys;

        //The users currently listened to.
        public IReadOnlyCollection<ulong> Users => _userDict.Keys;

        //The dictionary storing listeners.
        public SysLog Logger => _logger;

        public InteractionListenerService(IServiceProvider services) {
            _services = services;

            _contextualizerService = services.GetRequiredService<ContextualizerService>();

            _logger = SysLog.CreateAsync("Interactions").Result;

            _contextualizerService.MessageComponentInteraction += InteractionHandler;

            _timer = new Timer {
                Interval = PRUNE_INTERVAL,
                AutoReset = true,
                Enabled = true,
            };

            _timer.Elapsed += OnTimedEvent;
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e) {
            if (await TryRemoveOldListners())
                await _logger.Info(GetType(), $"Successfully removed old listener(s)!");
        }

        //Registres a new reaction listener to a message.
        public async Task<bool> RegisterNewListner(IInteractionListener listener) {
            //await _logger.Verbose(GetType(), $"Interaction registring");

            if (listener.ListenerType == ListenerType.Owned && listener.Target == null)
                throw new InvalidOperationException($"The listener is marked as Owned, but no owner was set.");

            if (listener.ListenerType == ListenerType.Hosted && listener.Owner == null)
                throw new InvalidOperationException($"The listener is marked as Hosted, but no host was set.");

            var message = listener.Message;
            var owner = listener.Owner;

            //Check cap.
            if (Count >= SOFT_CAP) {
                if (await TryRemoveOldListners()) {
                    await _logger.Verbose(GetType(),
                        $"Removed some expired listeners (New count: {_messageDict.Count})", parameterString: $"{message.Id}, {owner}");
                }

                if (Count >= HARD_CAP) {
                    await _logger.Error(GetType(),
                        $"Hard cap exceeded ({_messageDict.Count} >= {HARD_CAP})", parameterString: $"{message.Id}, {owner}");
                    return false;
                } else if (Count >= SOFT_CAP) {
                    await _logger.Warning(GetType(),
                        $"Soft cap exceeded ({_messageDict.Count} >= {SOFT_CAP})", parameterString: $"{message.Id}, {owner}");
                }
            }

            //Add the listener.
            if (_messageDict.TryAdd(message.Id, listener)) {

                //If the listener is owned.
                if (listener.ListenerType == ListenerType.Owned) {
                    if (_userDict.TryGetValue(listener.Target.Id, out var listener2)) await RemoveListner(listener2);

                    _userDict[listener.Target.Id] = listener;
                }

                //If the listener is hosted.
                else if (listener.ListenerType == ListenerType.Hosted) {
                    if (_hostDict.TryGetValue(listener.Owner.Id, out var listener2)) await RemoveListner(listener2);

                    _hostDict[listener.Owner.Id] = listener;
                }

                return true;
            } else {
                await _logger.Error(GetType(),
                    $"Listener already exists for message (New count: {_messageDict.Count})", parameterString: $"{message.Id}");
                return false;
            }
        }

        //Removes a listener.
        public async Task<bool> RemoveListner(IInteractionListener listener) {
            Preconditions.NotNull(listener, nameof(listener));
            Preconditions.NotNull(listener.Message, nameof(listener.Message));

            //Remove listener from the dictionary.
            if (_messageDict.Remove(listener.Message.Id)) {

                //Remove listener from the dict for listeners by user too.
                if (listener.ListenerType == ListenerType.Owned
                    && _userDict.TryGetValue(listener.Target.Id, out var listener2)
                    && listener == listener2) {
                    _userDict.Remove(listener.Owner.Id);
                }

                //Remove listener from hoste dict.
                else if (listener.ListenerType == ListenerType.Hosted
                    && _hostDict.TryGetValue(listener.Owner.Id, out var listener3)
                    && listener == listener3) {
                    _hostDict.Remove(listener.Owner.Id);
                }

                //Inform the listener that it is dead.
                await listener.Kill();

                return true;
            } else return false;
        }

        public async Task<bool> RemoveListner(ulong key)
            => (_messageDict.TryGetValue(key, out var listener) && await RemoveListner(listener));
        public Task<bool> RemoveListner(IMessage message)
            => RemoveListner(message.Id);

        //Removes old listeners.
        public async Task<bool> TryRemoveOldListners() {

            var old = _messageDict
                .Where(pair => pair.Value.IsExpired())
                .Select(pair => pair.Value);

            foreach (var listener in old) {
                await RemoveListner(listener);
            }

            var countRemoved = old.Count();

            return countRemoved > 0;
        }

        //Triggered when a ReactionAdded event occurs.
        //Finds and triggers correct listener if applicable.
        protected async Task InteractionHandler(MessageComponentParams par) {
            //Skip if reacted to by bot.
            if (par.User.IsBot || par.User.IsWebhook) return;

            //If the message isn't registred for listening, clear the components.
            if (!_messageDict.TryGetValue(par.Message.Id, out var listener)) {
                await par.Message.ModifyAsync(message => message.Components = new ComponentBuilder().Build());
                return;
            }

            var parameter = InteractionEventParameters.FromEvent(par);

            //Check age.
            if (listener.IsExpired()) {
                await RemoveListner(listener);
                await _logger.Debug(GetType(),
                    $"Listener is old (New count: {_messageDict.Count})", parameterString: $"{par.Message.Id}");
                return;
            }

            //If this user isn't allowed to use this message, tell them.
            if (listener.ListenerType == ListenerType.Owned && par.User.Id != listener.Target.Id) {
                var embed = EmbedUtils.StandardEmbed("Button pressed", par.User)
                    .WithColor(Color.Green)
                    .WithDescription(new StringBuilder()
                        .AppendLine($"You cannot interact with this message, only {listener.Target.Mention} can.")
                        .ToString());
                await par.MessageComponent.RespondAsync(embed: embed.Build(), ephemeral: true);
                return;
            }

            try {
                var res = await listener.RunAsync(parameter);

                if (!res.IsSuccess) {
                    await _logger.Debug(GetType(),
                        $"Listener ran unsuccessfully \"{res.ErrorReason}\" (New count: {_messageDict.Count})", parameterString: $"{par.Message.Id}");
                } //If it succeded.
                else await parameter.Interaction.DeferAsync();

                if (res.IsComplete) {
                    await _logger.Debug(GetType(),
                        $"Listener has been removed due to being complete (New count: {_messageDict.Count})", parameterString: $"{par.Message.Id}");

                    await RemoveListner(listener);
                }

                return;
            } catch (Exception e) {
                await RemoveListner(listener);
                await _logger.Error(GetType(),
                    $"Listener was removed due to throwing an uncought exception (New count: {_messageDict.Count})", e, parameterString: $"{par.Message.Id}");
                return;
            }
        }
    }
}