using System.Collections.Generic;
using System;
using Discord.WebSocket;
using Discord;
using Interaction.Types;

namespace Interaction.Parameter {
    //The parameters we send to a reaction listener when it is invoced.
    public class InteractionEventParameters : IInteractionEventParameters {
        //The custom id of the component.
        public string CustomId { get; }

        //The type of component that was used.
        public ComponentType Type { get; }

        //The component that was used.
        public IMessageComponent Component { get; }

        //The values of the component.
        public IReadOnlyCollection<string> Values { get; }

        //The user that did the interaction.
        public IUser User { get; }

        //The guild this happened in.
        public IGuild Guild { get; }

        //The channel the message belongs to.
        public IMessageChannel Channel { get; }

        //The message this happened to.
        public IMessage Message { get; }

        //The time this occured in.
        public DateTimeOffset Timestamp { get; }

        //The interaction object.
        public SocketMessageComponent Interaction { get; }
        private InteractionEventParameters(
            string CustomId, ComponentType Type, IMessageComponent Component,
            IReadOnlyCollection<string> Values,
            IUser User, IGuild Guild, IMessageChannel Channel, IMessage Message,
            DateTimeOffset Timestamp,
            SocketMessageComponent Interaction
        ) {
            this.CustomId = CustomId;
            this.Type = Type;
            this.Component = Component;
            this.Values = Values;
            this.User = User;
            this.Guild = Guild;
            this.Channel = Channel;
            this.Message = Message;
            this.Timestamp = Timestamp;
            this.Interaction = Interaction;
        }

        public static InteractionEventParameters FromEvent(MessageComponentParams par)
            => new(
                par.Data.CustomId,
                par.Data.Type,
                par.Component,
                par.Data.Values,
                par.User,
                par.Guild,
                par.Channel,
                par.Message,
                par.Timestamp,
                par.MessageComponent
            );
    }
}