using Discord.WebSocket;
using Discord;


namespace Interaction.TestingGrounds.Parameter {
    public class InteractionParams : EventParam, IInteractionParams {
        public SocketInteraction Interaction { get; set; }

        public ISocketMessageChannel Channel => Interaction.Channel;
        public SocketUser User => Interaction.User;
        public InteractionType Type => Interaction.Type;
        public string Token => Interaction.Token;
        public IDiscordInteractionData Data => Interaction.Data;
        public bool IsValidToken => Interaction.IsValidToken;
    }
}