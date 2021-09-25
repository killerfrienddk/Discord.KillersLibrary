using System.Collections.Generic;
using System;
using Discord.WebSocket;
using Discord;

namespace Interaction.Parameter {
    //The parameters we send to a reaction listener when it is invoked.
    public interface IInteractionEventParameters {

        //The custom id of the component.
        string CustomId { get; }

        //The type of component that was used.
        ComponentType Type { get; }

		//The component that was used.
		IMessageComponent Component { get; }


		//The values of the component.
		IReadOnlyCollection<string> Values { get; }

		//The user that did the interaction.
		IUser User { get; }

		//The guild this happened in.
		IGuild Guild { get; }

		//The channel the message belongs to.
		IMessageChannel Channel { get; }

		//The message this happened to.
		IMessage Message { get; }

		//The time this occured in.
		DateTimeOffset Timestamp { get; }

		//The interaction object.
		SocketMessageComponent Interaction { get; }
    }
}