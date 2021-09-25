using System;
using System.Threading.Tasks;

// ---- // 

using Discord.Commands;
using Discord;

// ---- //


namespace Interaction.Types.TypeReaders {

	/*
		TypeReader for Emote objects.
	*/
	public class EmoteTypeReader : TypeReader {

		public EmoteTypeReader() { }

		public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services) {
			await Task.CompletedTask;

			if(Emote.TryParse(input, out var emote))
				return TypeReaderResult.FromSuccess(new TypeReaderValue(emote, 1.0f));

			return TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {typeof(Emote).Name}.");
		}
		
	}

	
	/*
		TypeReader for Emoji objects.
	*/
	public class EmojiTypeReader : TypeReader {

		public EmojiTypeReader() { }

		public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services) {
			await Task.CompletedTask;

			try {
				return TypeReaderResult.FromSuccess(new TypeReaderValue(new Emoji(input), 1.0f));
			} catch(Exception e) {
				return TypeReaderResult.FromError(e);
			}
		}
		
	}


	/*
		TypeReader for IEmote objects.
	*/
	public class EmoteInterfaceTypeReader : TypeReader {

		public EmoteInterfaceTypeReader() { }

		public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services) {
			await Task.CompletedTask;

			try {
					
				if(Emote.TryParse(input, out var emote))
					return TypeReaderResult.FromSuccess(new TypeReaderValue(emote, 1.0f));

				return TypeReaderResult.FromSuccess(new TypeReaderValue(new Emoji(input), 1.0f));
			} catch(Exception e) {
				return TypeReaderResult.FromError(e);
			}
		}
		
	}
}