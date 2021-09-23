using System.Threading.Tasks;
using System.IO;
using System;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;
using Discord;

namespace KillersLibrary {
    public class CommonService {
        internal CommonService() { }
        static CommonService() { }

        private static readonly CommonService _instance = new();

        public static CommonService Instance {
            get { return _instance; }
        }

        #region Other
        /// <summary>
        ///     Gets the DiscordID depending on which of the two inputs are <see langword="null"/>.
        /// </summary>
        /// <param name="context">The <see cref="SocketCommandContext"/> is used to send normal commands.</param>
        /// <param name="command">The <see cref="SocketSlashCommand"/> is used to send slash commands.</param>
        /// <returns>A DiscordID from either <see cref="SocketCommandContext"/> or <see cref="SocketSlashCommand"/> depending on which is <see langword="null"/>.</returns>
        public virtual ulong GetDiscordID(SocketCommandContext context = null, SocketSlashCommand command = null) {
            ContextAndCommandIsNullCheck(context, command);
            if (context == null) return command.User.Id;
            else return context.User.Id;
        }

        /// <summary>
        ///     Gets the UserID depending on which of the two inputs are <see langword="null"/>.
        /// </summary>
        /// <param name="context">The <see cref="SocketCommandContext"/> is used to send normal commands.</param>
        /// <param name="command">The <see cref="SocketSlashCommand"/> is used to send slash commands.</param>
        /// <returns>A UserID from either <see cref="SocketCommandContext"/> or <see cref="SocketSlashCommand"/> depending on which is <see langword="null"/>.</returns>
        [Obsolete("This method will soon be deprecated and will be removed in future versions. Please use the new GetDiscordID instead", true)]
        public virtual ulong GetUserID(SocketCommandContext context = null, SocketSlashCommand command = null) {
            ContextAndCommandIsNullCheck(context, command);
            if (context == null) return command.User.Id;
            else return context.User.Id;
        }

        /// <summary>
        ///     Gets the GuildID depending on which of the two inputs are <see langword="null"/>.
        /// </summary>
        /// <param name="context">The <see cref="SocketCommandContext"/> is used to send normal commands.</param>
        /// <param name="command">The <see cref="SocketSlashCommand"/> is used to send slash commands.</param>
        /// <returns>A GuildID from either <see cref="SocketCommandContext"/> or <see cref="SocketSlashCommand"/> depending on which is <see langword="null"/>.</returns>
        public virtual ulong GetGuildID(SocketCommandContext context = null, SocketSlashCommand command = null) {
            ContextAndCommandIsNullCheck(context, command);
            if (context == null) return ((SocketGuildUser)command.User).Guild.Id;
            else return context.Guild.Id;
        }

        /// <summary>
        ///     Gets the AuthorID depending on which of the two inputs are <see langword="null"/>.
        /// </summary>
        /// <param name="context">The <see cref="SocketCommandContext"/> is used to send normal commands.</param>
        /// <param name="command">The <see cref="SocketSlashCommand"/> is used to send slash commands.</param>
        /// <returns>A AuthorID from either <see cref="SocketCommandContext"/> or <see cref="SocketSlashCommand"/> depending on which is <see langword="null"/>.</returns>
        public virtual ulong GetAuthorID(SocketCommandContext context = null, SocketSlashCommand command = null) {
            ContextAndCommandIsNullCheck(context, command);
            if (context == null) return command.User.Id;
            else return context.Message.Author.Id;
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentException"/> if both the <see cref="SocketCommandContext"/> and the <see cref="SocketSlashCommand"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="context">The <see cref="SocketCommandContext"/>.</param>
        /// <param name="command">The <see cref="SocketSlashCommand"/>.</param>
        public virtual void ContextAndCommandIsNullCheck(SocketCommandContext context = null, SocketSlashCommand command = null) {
            if (context == null && command == null) throw new ArgumentException("Both the context and the command are empty. Please fill one of the parameters.");
        }

        #region Responses
        /// <summary>
        ///     Sends a file using <see cref="ISocketMessageChannel"/> depending on which of the two inputs are <see langword="null"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> is the image data you want to send.</param>
        /// <param name="filename">The filename of the attachment.</param>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="context">The <see cref="SocketCommandContext"/> is used to send normal commands.</param>
        /// <param name="command">The <see cref="SocketSlashCommand"/> is used to send slash commands.</param>
        public virtual async Task MakeFileResponseAsync(Stream stream, string filename, string text = null, SocketCommandContext context = null, SocketSlashCommand command = null) {
            ContextAndCommandIsNullCheck(context, command);
            if (context == null) await command.FollowupWithFileAsync(text, stream, filename);
            else await context.Channel.SendFileAsync(stream, filename, text);
        }

        /// <summary>
        ///     Sends a file using <see cref="ISocketMessageChannel"/> depending on which of the two inputs are <see langword="null"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> is the image data you want to send.</param>
        /// <param name="filename">The filename of the attachment.</param>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="context">The <see cref="SocketCommandContext"/> is used to send normal commands.</param>
        /// <param name="command">The <see cref="SocketSlashCommand"/> is used to send slash commands.</param>
        [Obsolete("This method will soon be deprecated and will be removed in future versions. Please use the new MakeFileResponseAsync instead", true)]
        public virtual async Task MakeFileResponse(Stream stream, string filename, string text = null, SocketCommandContext context = null, SocketSlashCommand command = null) {
            await MakeFileResponseAsync(stream, filename, text, context, command);
        }

        /// <summary>
        ///     Sends a file using <see cref="ISocketMessageChannel"/> depending on which of the two inputs are <see langword="null"/>.
        /// </summary>
        /// <param name="text">The text of the message to be sent.</param>
        /// <param name="embed">A single embed to send with this response. If this is passed alongside an array of embeds, the single embed will be ignored.</param>
        /// <param name="component">A <see cref="MessageComponent"/> to be sent with this response</param>
        /// <param name="context">The <see cref="SocketCommandContext"/> is used to send normal commands.</param>
        /// <param name="command">The <see cref="SocketSlashCommand"/> is used to send slash commands.</param>
        /// <returns>
        /// A <see cref="RestUserMessage"/>
        /// </returns>
        public virtual async Task<RestUserMessage> MakeResponse(string text = null, Sticker[] stickers = null, Embed embed = null, Embed[] embeds = null, MessageComponent component = null, bool disregardArgumentExceptions = false, SocketCommandContext context = null, SocketSlashCommand command = null) {
            ContextAndCommandIsNullCheck(context, command);
            if (context == null) {
                if (!disregardArgumentExceptions && stickers != null) throw new ArgumentException("Unfortunately FollowupAsync does not support stickers at this time.");
                return await command.FollowupAsync(text ?? " ", embed: embed, embeds: embeds, component: component);
            }
            else {
                if (!disregardArgumentExceptions && embeds != null) throw new ArgumentException("Unfortunately SendMessageAsync does not support multiple embeds at this time.");
                return await context.Channel.SendMessageAsync(text ?? " ", embed: embed, component: component, stickers: stickers);
            }
        }
        #endregion
        #endregion
    }
}
