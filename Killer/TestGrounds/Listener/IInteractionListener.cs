using System.Threading.Tasks;
using System;
using Discord;
using Interaction.TestingGrounds.Listener.Enums;

namespace Interaction.TestingGrounds.Listener {
	//Listenes to reactions added to / removed from a message.	
	public interface IInteractionListener {
		//The type of listener this is.
		ListenerType ListenerType { get; }

		TimeSpan MaxLifespan { get; } 
		TimeSpan MaxIdle { get; }
		TimeSpan Cooldown { get; }
	
		DateTimeOffset Timestamp { get; } 
		DateTimeOffset LastAction { get; }

		IUserMessage Message { get; }
		IMessageChannel Channel { get; }
		
		IUser Target { get; }
		IUser Owner { get; }

		InteractionListenerService Service { get; }
		ListenerStatus Status { get; }

		DateTimeOffset Deadline { get; }

		//Checks if the listener has expired.
		bool IsExpired();

		//Runs when a reaction event has been fired.
		Task<IInteractionResult> RunAsync(IInteractionEventParameters par);

		//Connects the listener to a ReactionListenerService.
		Task<bool> Connect(InteractionListenerService service);

		//Kills off the listener.
		Task<bool> Kill();
	}
}